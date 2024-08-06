using DAL.Entities.IdentityModule;
using DAL.Entities.IdentityModule.Helpers;
using DPL.Services.Contract;
using DPL.Services.Helpers;
using DPL.ViewModels.Auth.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DPL.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly ITokenService _tokenService;
		private readonly ISmsSender _smsSender;

		public AccountController(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IEmailSender emailSender,
			ITokenService tokenService,
			ISmsSender smsSender)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_tokenService = tokenService;
			_smsSender = smsSender;
		}

		#region Register

		[HttpGet]
		public IActionResult Register() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterVM input)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser()
				{
					UserName = input.UserName,
					FirstName = input.FirstName,
					LastName = input.LastName,
					Email = input.Email,
					PhoneNumber = $"+2{input.PhoneNumber}",
					AgreeOnTerms = input.AgreeOnTerms
				};

				var result = await _userManager.CreateAsync(user, input.Password);
				await _userManager.AddToRoleAsync(user, RoleConstants.User);

				if (result.Succeeded)
					return RedirectToAction(nameof(Login));

				foreach (var error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(input);
		}

		#endregion

		#region Login 

		[HttpGet]
		public IActionResult Login() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginVM input)
		{
			if (ModelState.IsValid)
			{
				var user = input.EmailOrUserName.Contains('@')
					? await _userManager.FindByEmailAsync(input.EmailOrUserName)
					: await _userManager.FindByNameAsync(input.EmailOrUserName);

				if (user is not null)
				{
					var isCorrectPassword = await _userManager.CheckPasswordAsync(user, input.Password);
					if (isCorrectPassword)
					{
						#region Using "Identity.Application" Scheme
						/*
						var result = await _signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, false);

						if (result.IsLockedOut)
							ModelState.AddModelError(string.Empty, "Your Account Is LockedOut");
						//else if (result.IsNotAllowed)
						//	ModelState.AddModelError(string.Empty, "Your Account Is Not Confirmed");
						else
							return RedirectToAction(nameof(HomeController.Index), "Home");
						*/
						#endregion

						#region Using "JWT" Scheme

						var result = await _signInManager.CheckPasswordSignInAsync(user, input.Password, false);

						if (result.IsLockedOut)
							ModelState.AddModelError(string.Empty, "Your Account Is LockedOut");

						//else if (result.IsNotAllowed)
						//	ModelState.AddModelError(string.Empty, "Your Account Is Not Confirmed");

						else
						{
							var token = await _tokenService.GenerateTokenAsync(user);

							if (input.RememberMe)
							{
								var cookieOptions = new CookieOptions()
								{
									HttpOnly = true,
									Expires = DateTimeOffset.UtcNow.AddDays(14)
								};
								Response.Cookies.Append(JwtSettings.JwtTokenKey, token, cookieOptions);
							}
							else
								Response.Cookies.Append(JwtSettings.JwtTokenKey, token); // Till Session Ends

							return RedirectToAction(nameof(HomeController.Index), "Home");
						}

						#endregion
					}
					else
						ModelState.AddModelError(string.Empty, "Invalid Email/UserName Or Password");
				}
				else
					ModelState.AddModelError(string.Empty, "Invalid Email/UserName Or Password");
			}
			return View(input);
		}


		[HttpGet]
		public IActionResult LoginWithGoogle()
		{
			var properties = new AuthenticationProperties()
			{
				RedirectUri = Url.Action(nameof(GoogleResponse))
			};
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		public async Task<IActionResult> GoogleResponse()
		{
			var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
			if (result.Succeeded)
			{
				var emailClaim = result.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
				if (emailClaim is not null)
				{
					var email = emailClaim.Value;
					var user = await _userManager.FindByEmailAsync(email);
					if (user is not null)
					{
						var token = await _tokenService.GenerateTokenAsync(user);
						var cookieOptions = new CookieOptions()
						{
							HttpOnly = true,
							Expires = DateTimeOffset.UtcNow.AddDays(14)
						};
						Response.Cookies.Append(JwtSettings.JwtTokenKey, token, cookieOptions);
						return RedirectToAction(nameof(HomeController.Index), "Home");
					}
				}
			}
			return Unauthorized();
		}

		#endregion

		#region LogOut

		[HttpGet]
		public /*async Task<IActionResult>*/ async Task<IActionResult> LogOutAsync()
		{
			#region Using "Identity.Application" Scheme

			/*
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(HomeController.Index), "Home"); 
			*/

			#endregion

			#region Using JWT

			// To Remove Jwt From Cookie Storage
			if (Request.Cookies.ContainsKey(JwtSettings.JwtTokenKey))
				Response.Cookies.Delete(JwtSettings.JwtTokenKey);

			// To Remove "Identity.External" From Cookie Storage
			await _signInManager.SignOutAsync();

			return RedirectToAction(nameof(HomeController.Index), "Home");

			#endregion

		}

		#endregion

		#region ForgetPassword

		[HttpGet]
		public IActionResult ForgetPassword() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgetPassword(ForgerPasswordVM input)
		{
			if (ModelState.IsValid)
			{
				var existingUser = await _userManager.FindByEmailAsync(input.RecoveryEmail);

				if (existingUser is not null)
				{
					var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

					var resetPasswordLink = Url.Action(
						action: nameof(ResetPassword),
						controller: "Account",
						values: new { Email = input.RecoveryEmail, Token = token },
						protocol: Request.Scheme,
						host: Request.Host.Value
						);

					var email = new Email()
					{
						To = input.RecoveryEmail,
						Subject = "Reset Password",
						Body = resetPasswordLink!
					};
					await _emailSender.SendEmailAsync(email);

					if (existingUser.PhoneNumber is not null)
						await _smsSender.SendSmsAsync(new SmsMessage()
						{
							Message = "Password Reset Link Has Been Sent To Your Mail",
							PhoneNumber = existingUser.PhoneNumber
						});

					return RedirectToAction(nameof(CheckYourInbox));
				}
			}
			ModelState.AddModelError(string.Empty, "Invalid Email Address");
			return View(input);
		}

		[HttpGet]
		public IActionResult CheckYourInbox() => View();
		#endregion

		#region Reset Password

		[HttpGet]
		public IActionResult ResetPassword(string email, string token)
		{
			TempData["Email"] = email;
			TempData["Token"] = token;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordVM input)
		{
			if (ModelState.IsValid)
			{
				var email = (TempData["Email"] as string)!;
				var token = (TempData["Token"] as string)!;

				var user = await _userManager.FindByEmailAsync(email);
				if (user is not null)
				{
					var result = await _userManager.ResetPasswordAsync(user, token, input.NewPassword);

					if (result.Succeeded)
						return RedirectToAction(nameof(Login));
					else
					{
						TempData.Keep("Email");
						TempData.Keep("Token");

						foreach (var error in result.Errors)
							ModelState.AddModelError(string.Empty, error.Description);
					}
				}
				else
					ModelState.AddModelError(string.Empty, "Invalid Url");
			}
			return View();
		}

		#endregion
	}
}
