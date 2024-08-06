using AutoMapper;
using DAL.Entities.IdentityModule;
using DAL.Entities.IdentityModule.Helpers;
using DPL.ViewModels.Auth.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DPL.Controllers
{
	[Authorize(Roles = RoleConstants.Admin)]
	public class UsersController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IMapper _mapper;
		private readonly RoleManager<IdentityRole> _roleManager;

		public UsersController(UserManager<ApplicationUser> userManager,
			IMapper mapper,
			RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_mapper = mapper;
			_roleManager = roleManager;
		}

		public async Task<IActionResult> Index(string searchInput)
		{
			var model = Enumerable.Empty<UserVM>();

			if (string.IsNullOrEmpty(searchInput))
			{
				var users = _userManager.Users
					.Select(user => new UserVM()
					{
						Id = user.Id,
						FirstName = user.FirstName,
						LastName = user.LastName,
						UserName = user.UserName ?? string.Empty,
						Email = user.Email ?? string.Empty,
						Roles = _userManager.GetRolesAsync(user).Result
					})
					.ToList();

				model = users;

			}

			else
			{
				var user = await _userManager.FindByNameAsync(searchInput);
				if (user is not null)
				{
					var userVM = new UserVM()
					{
						Id = user.Id,
						FirstName = user.FirstName,
						LastName = user.LastName,
						UserName = user.UserName ?? string.Empty,
						Email = user.Email ?? string.Empty,
						Roles = _userManager.GetRolesAsync(user).Result
					};
					model = new List<UserVM>() { userVM };
				}
			}
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Details(string? id, string viewName = nameof(Details))
		{
			if (id is null)
				return BadRequest();

			var user = await _userManager.FindByIdAsync(id);

			if (user is null)
				return NotFound();

			var roles = await _userManager.GetRolesAsync(user);
			var model = _mapper.Map<UserVM>(user);
			model.Roles = roles;
			return View(viewName, model);
		}

		[HttpGet]
		public async Task<IActionResult> Edit([FromRoute] string? id)
			=> await Details(id, nameof(Edit));

		[HttpPost]
		public async Task<IActionResult> Edit([FromRoute] string id, UserVM input)
		{
			if (!string.Equals(id, input.Id))
				return BadRequest();
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(input.Id);
				if (user is null)
					return BadRequest();

				user.FirstName = input.FirstName ?? string.Empty;
				user.LastName = input.LastName ?? string.Empty;
				user.PhoneNumber = input.PhoneNumber;

				var result = await _userManager.UpdateAsync(user);
				if (result.Succeeded)
					return RedirectToAction(nameof(Index));

				foreach (var error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(input);
		}

		[HttpGet]
		public async Task<IActionResult> Delete([FromRoute] string? id)
			=> await Details(id, nameof(Delete));

		[HttpPost]
		public async Task<IActionResult> Delete([FromRoute] string id, UserVM input)
		{

			if (!string.Equals(id, input.Id))
				return BadRequest();

			var user = await _userManager.FindByIdAsync(id);
			if (user is null)
				return BadRequest();

			var result = await _userManager.DeleteAsync(user);
			if (result.Succeeded)
				return RedirectToAction(nameof(Index));
			else
			{
				ModelState.AddModelError(string.Empty, "Unexpected Error");
				return View(input);
			}
		}
	}
}
