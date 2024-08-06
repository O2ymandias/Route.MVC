using AutoMapper;
using BLL.Interfaces;
using DAL.Entities.BusinessModule;
using DAL.Entities.IdentityModule.Helpers;
using DPL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPL.Controllers
{
	[Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.User}")]
	public class DepartmentsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ILogger<DepartmentsController> _logger;
		private readonly IWebHostEnvironment _environment;

		public DepartmentsController(IUnitOfWork unitOfWork,
			IMapper mapper,
			ILogger<DepartmentsController> logger,
			IWebHostEnvironment environment)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_logger = logger;
			_environment = environment;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var departmentRepo = _unitOfWork.Repository<Department>();
			var departments = await departmentRepo.GetAllAsync();
			var model = _mapper.Map<IReadOnlyList<DepartmentVM>>(departments);
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Details(int? id, string viewName = nameof(Details))
		{
			if (id is null)
				return BadRequest();

			var department = await _unitOfWork.Repository<Department>().GetAsync(id.Value);
			if (department is null)
				return NotFound();

			var model = _mapper.Map<DepartmentVM>(department);
			return View(viewName, model);
		}


		[HttpGet]
		public IActionResult Create()
			=> View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DepartmentVM input)
		{
			if (ModelState.IsValid)
			{
				var department = _mapper.Map<Department>(input);
				var departmentRepo = _unitOfWork.Repository<Department>();

				departmentRepo.Add(department);
				try
				{
					var result = await _unitOfWork.CompleteAsync();
					if (result > 0)
						TempData.Add("Message", $"Department: {department.Name} Has Been Added Successfully");
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					if (_environment.IsDevelopment())
					{
						_logger.LogError(ex, "An Error Has Occurred While Creating Department");
						ModelState.AddModelError(string.Empty, ex.Message);
					}
					else
						ModelState.AddModelError(string.Empty, "Unexpected Error !!");
				}
			}

			return View(input);
		}


		[HttpGet]
		public async Task<IActionResult> Edit(int? id)
			=> await Details(id, nameof(Edit));

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DepartmentVM input, [FromRoute] int id)
		{
			if (id != input.Id)
				return BadRequest();

			if (ModelState.IsValid)
			{
				var departmentRepo = _unitOfWork.Repository<Department>();
				var department = _mapper.Map<Department>(input);

				departmentRepo.Update(department);
				try
				{
					var result = await _unitOfWork.CompleteAsync();
					if (result > 0)
						TempData.Add("Message", $"Department: {department.Name} Has Been Updated Successfully");
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					if (_environment.IsDevelopment())
					{
						_logger.LogError(ex, "An Error Has Occurred While Editing Department");
						ModelState.AddModelError(string.Empty, ex.Message);
					}
					else
						ModelState.AddModelError(string.Empty, "Unexpected Error !!");
				}
			}
			return View(input);
		}


		[HttpGet]
		public async Task<IActionResult> Delete(int? id)
			=> await Details(id, nameof(Delete));

		[HttpPost]
		public async Task<IActionResult> Delete(DepartmentVM input, [FromRoute] int id)
		{
			if (input.Id != id)
				return BadRequest();

			var department = _mapper.Map<Department>(input);
			_unitOfWork.Repository<Department>().Delete(department);

			try
			{
				var result = await _unitOfWork.CompleteAsync();
				if (result > 0)
					TempData.Add("Message", $"Department: {department.Name} Has Been Deleted Successfully");
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				if (_environment.IsDevelopment())
				{
					_logger.LogError(ex, "An Error Has Occurred While Deleting Department");
					ModelState.AddModelError(string.Empty, ex.Message);
				}
				else
					ModelState.AddModelError(string.Empty, "Unexpected Error !!");
			}
			return View(input);
		}
	}
}

