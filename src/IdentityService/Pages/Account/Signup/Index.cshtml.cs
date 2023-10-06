using System.Diagnostics;
using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Signup
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public Index(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public SignupViewModel Input { get; set; }


        [BindProperty]
        public bool RegisterSuccess { get; set; }



        public IActionResult OnGet(string returnUrl)
        {
            Input = new SignupViewModel
            {
                ReturnUrl = returnUrl,
            };

            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            if (Input.Button != "signup") return Redirect("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddClaimsAsync(user, new Claim[]{
                        new Claim(JwtClaimTypes.GivenName, Input.FirstName),
                        new Claim(JwtClaimTypes.FamilyName, Input.LastName)
                    });

                    RegisterSuccess = true;
                }
                else
                {
                    RegisterSuccess = false;
                }
            }

            return Page();
        }
    }
}
