using AutoMapper;
using DAL.Entities.IdentityModule.Helpers;
using DPL.ViewModels.Auth.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DPL.Controllers
{
	[Authorize(Roles = RoleConstants.Admin)]
	public class RolesController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMapper _mapper;

		public RolesController(RoleManager<IdentityRole> roleManager,
			IMapper mapper)
		{
			_roleManager = roleManager;
			_mapper = mapper;
		}
		public async Task<IActionResult> Index(string searchInput)
		{
			var model = Enumerable.Empty<RoleVM>();

			if (string.IsNullOrEmpty(searchInput))
			{
				var roles = await _roleManager.Roles.ToListAsync();
				model = _mapper.Map<IEnumerable<RoleVM>>(roles);
			}
			else
			{
				var role = await _roleManager.FindByNameAsync(searchInput);
				if (role is not null)
					model = new List<RoleVM>() { _mapper.Map<RoleVM>(role) };
			}
			return View(model);
		}


		public IActionResult Create()
			 => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateRoleVM input)
		{
			if (ModelState.IsValid)
			{
				var role = new IdentityRole() { Name = input.RoleName };
				var result = await _roleManager.CreateAsync(role);
				if (result.Succeeded)
					return RedirectToAction(nameof(Index));

				foreach (var error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(input);
		}


		public async Task<IActionResult> Details(string? id, string viewName = nameof(Details))
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var role = await _roleManager.FindByIdAsync(id);
			if (role is null)
				return NotFound();

			var model = new RoleVM()
			{
				Id = role.Id,
				Name = role.Name ?? string.Empty,
			};
			return View(viewName, model);
		}


		[HttpGet]
		public async Task<IActionResult> Edit(string? id)
			=> await Details(id, nameof(Edit));

		[HttpPost]
		public async Task<IActionResult> Edit([FromRoute] string? id, RoleVM input)
		{
			if (id != input.Id)
				return BadRequest();

			if (ModelState.IsValid)
			{
				var role = await _roleManager.FindByIdAsync(id);
				if (role is null)
					return BadRequest();

				role.Name = input.Name;
				var result = await _roleManager.UpdateAsync(role);

				if (result.Succeeded)
					return RedirectToAction(nameof(Index));

				foreach (var error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(input);
		}


		[HttpGet]
		public async Task<IActionResult> Delete(string? id)
			=> await Details(id, nameof(Delete));

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete([FromRoute] string id, RoleVM input)
		{
			if (id != input.Id)
				return BadRequest();

			var role = await _roleManager.FindByIdAsync(id);

			if (role is null)
				return BadRequest();

			var result = await _roleManager.DeleteAsync(role);
			if (result.Succeeded)
				return RedirectToAction(nameof(Index));

			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);
			return View(result);
		}

	}
}
