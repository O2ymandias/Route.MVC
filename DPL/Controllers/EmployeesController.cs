using AutoMapper;
using BLL.Interfaces;
using BLL.Specifications.EmployeeSpecs;
using DAL.Entities.BusinessModule;
using DAL.Entities.IdentityModule.Helpers;
using DPL.Helpers;
using DPL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPL.Controllers
{
	[Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.User}")]
	public class EmployeesController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _environment;
		private readonly ILogger<EmployeesController> _logger;

		public EmployeesController(IUnitOfWork unitOfWork,
			IMapper mapper,
			IWebHostEnvironment environment,
			ILogger<EmployeesController> logger)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_environment = environment;
			_logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string? searchInput)
		{
			var employeeRepo = _unitOfWork.Repository<Employee>();
			var specs = new EmployeeSpecifications(searchInput);
			var employees = await employeeRepo.GetAllWithSpecs(specs);
			var model = _mapper.Map<IReadOnlyList<EmployeeVm>>(employees);
			return View(model);
		}

		[HttpGet]
		public IActionResult Create() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(EmployeeVm input)
		{
			if (ModelState.IsValid)
			{
				var employeeRepo = _unitOfWork.Repository<Employee>();

				if (input.Image is not null)
					input.ImageName = DocumentSettings.UploadFile(input.Image, "images");

				var employee = _mapper.Map<Employee>(input);

				employeeRepo.Add(employee);
				try
				{
					var result = await _unitOfWork.CompleteAsync();
					if (result > 0)
						TempData.Add("Message", $"{employee.Name} Has Been Added Successfully");
					// Passing Data From Current Request "/Employees/Create" To Subsequent Request "/Employees/Index"
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{

					if (_environment.IsDevelopment())
					{
						_logger.LogError(ex, "An Error Has Occurred While Creating Employee");
						ModelState.AddModelError(string.Empty, ex.Message);
					}
					else
						ModelState.AddModelError(string.Empty, "Unexpected Error !!");
				}
			}
			return View(input);
		}

		[HttpGet]
		public async Task<IActionResult> Details(int? id, string viewName = nameof(Details))
		{
			if (!id.HasValue)
				return BadRequest();

			var specs = new EmployeeSpecifications(id.Value);
			var employeeRepo = _unitOfWork.Repository<Employee>();
			var employee = await employeeRepo.GetWithSpecs(specs);

			if (employee is null)
				return NotFound();

			var model = _mapper.Map<EmployeeVm>(employee);
			return View(viewName, model);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int? id) => await Details(id, nameof(Edit));

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EmployeeVm input, [FromRoute] int id)
		{
			if (id != input.Id)
				return BadRequest();

			if (ModelState.IsValid)
			{
				var employeeRepo = _unitOfWork.Repository<Employee>();

				if (input.ImageName is not null) // Already Has An Image At Database
				{
					if (input.Image is not null) // Update Of Image
					{
						DocumentSettings.DeleteFile("images", input.ImageName); // Delete Old Image From App
						input.ImageName = DocumentSettings.UploadFile(input.Image, "images"); // Adding New Image To App, And Updating ImageName
					}
				}
				else // Doesn't Have An Image At Database
				{
					if (input.Image is not null)
						input.ImageName = DocumentSettings.UploadFile(input.Image, "images");
				}


				var employee = _mapper.Map<Employee>(input);

				employeeRepo.Update(employee);
				try
				{
					var result = await _unitOfWork.CompleteAsync();
					if (result > 0)
						TempData.Add("Message", $"{employee.Name} Has Been Updated Successfully");
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					if (_environment.IsDevelopment())
					{
						_logger.LogError(ex, "An Error Has Occurred While Editing Employee");
						ModelState.AddModelError(string.Empty, ex.Message);
					}
					else
						ModelState.AddModelError(string.Empty, "Unexpected Error !!");
				}
			}
			return View(input);
		}

		[HttpGet]
		public async Task<IActionResult> Delete(int? id) => await Details(id, nameof(Delete));

		[HttpPost]
		public async Task<IActionResult> Delete(EmployeeVm input, [FromRoute] int id)
		{
			if (input.Id != id)
				return BadRequest();

			var employee = _mapper.Map<Employee>(input);
			_unitOfWork.Repository<Employee>().Delete(employee);

			try
			{
				var result = await _unitOfWork.CompleteAsync();
				if (result > 0)
				{
					TempData.Add("Message", $"{employee.Name} Has Been Deleted Successfully");
					if (input.ImageName is not null)
						DocumentSettings.DeleteFile("images", input.ImageName);
				}
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				if (_environment.IsDevelopment())
				{
					_logger.LogError(ex, "An Error Has Occurred While Deleting Employee");
					ModelState.AddModelError(string.Empty, ex.Message);
				}
				else
					ModelState.AddModelError(string.Empty, "Unexpected Error !!");
			}
			return View(input);
		}

		public IActionResult CheckSalary(decimal salary)
		{
			return salary >= 5000
				? Json(true)
				: Json(false);
		}
	}
}
