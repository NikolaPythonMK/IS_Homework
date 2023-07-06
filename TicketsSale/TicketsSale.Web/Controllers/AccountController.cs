using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TicketsSale.Domain.DTO;
using TicketsSale.Domain.Identity;

namespace TicketsSale.Web.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<TicketsSaleApplicationUser> userManager;
        private readonly SignInManager<TicketsSaleApplicationUser> signInManager;
        public static int AccountsCreatedAtRuntime;
        private object lockObject = new object();


        public AccountController(UserManager<TicketsSaleApplicationUser> userManager,SignInManager<TicketsSaleApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            AccountsCreatedAtRuntime = 0;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            UserRegistrationDTO registration_model = new UserRegistrationDTO();
            return View(registration_model);
        }



        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDTO request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new TicketsSaleApplicationUser
                    {
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        ShoppingCart = new Domain.DomainModels.ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        TicketsSaleApplicationUser app_user = await userManager.FindByEmailAsync(request.Email);
                        await userManager.AddToRoleAsync(app_user, "Admin");

                        /*lock (lockObject)
                        {
                            if (AccountsCreatedAtRuntime == 0)
                            {
                                userManager.AddToRoleAsync(app_user, "Admin");
                                AccountsCreatedAtRuntime++;
                            }
                            else
                            {

                                userManager.AddToRoleAsync(app_user, "User");
                            }

                        }*/

                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDTO model = new UserLoginDTO();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Tickets");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }



        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            ICollection<TicketsSaleApplicationUser> Users = userManager.Users.ToList();
            UserToRole model = new UserToRole
            {
                UserEmails = Users
            };
            model.UserRoles.Add("Admin");
            model.UserRoles.Add("User");

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRole(UserToRole model)
        {
            TicketsSaleApplicationUser user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                var RoleToRemove = string.Join(" ", await userManager.GetRolesAsync(user));
                await userManager.RemoveFromRoleAsync(user, RoleToRemove);
                await userManager.AddToRoleAsync(user, model.SelectedRole);
                return RedirectToAction("Dashboard");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\Files\\{file.FileName}";          
            List<UserDTO> users = new List<UserDTO>();
            bool status = true;

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            string pathToFile = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            using (var stream = System.IO.File.Open(pathToFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new UserDTO
                        {
                            Email = reader.GetValue(0).ToString(),
                            PhoneNumber = "This user has not provided a phone number.",
                            NormalizedUserName = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            Role = reader.GetValue(2).ToString()
                        });
                    }
                }
            }

            foreach (UserDTO u in users)
            {
                var userCheck = await userManager.FindByEmailAsync(u.Email);
                if (userCheck == null)
                {
                    var user = new TicketsSaleApplicationUser
                    {
                        UserName = u.Email,
                        NormalizedUserName = u.Email,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        ShoppingCart = new Domain.DomainModels.ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, u.Password);
                    if (result.Succeeded)
                    {
                        TicketsSaleApplicationUser usr = await userManager.FindByEmailAsync(u.Email);
                        await userManager.AddToRoleAsync(usr, u.Role);
                    }
                    else
                    {
                        status = false;
                    }
                }
            }

            if (status == true)
            {
                return RedirectToAction("Dashboard");
            }

            return NotFound();
        }
    }
}
