using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Booker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Booker.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<BookerUser> _userManager;
        private readonly SignInManager<BookerUser> _signInManager;

        public IndexModel(
            UserManager<BookerUser> userManager,
            SignInManager<BookerUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string LastName { get; set; }
        public string FirstName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "FristName ")]
            public string NewFirstName { get; set; }
            [Display(Name = "LastName ")]
            public string NewLastName { get; set; }

        }

        private async Task LoadAsync(BookerUser user)
        {
            var firstName =  user.FirstName;
            var lastName = user.LastName;
         
            FirstName = firstName;
            LastName = lastName;
            Input = new InputModel
            {
                NewFirstName = firstName,
                NewLastName = lastName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var lastName = user.LastName;
            if (Input.NewLastName != lastName)
            {
               user.LastName = Input.NewLastName;
                var setLastNameResult = await _userManager.UpdateAsync(user);
                if (!setLastNameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set lastname.";
                    return RedirectToPage();
                }
            }

            var firstName = user.FirstName;
            if (Input.NewFirstName != firstName)
            {
                user.FirstName = Input.NewFirstName;
                var setLastNameResult = await _userManager.UpdateAsync(user);
                if (!setLastNameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set firstname.";
                    return RedirectToPage();
                }
            }


            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
