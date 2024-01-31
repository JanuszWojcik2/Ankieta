// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Info.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Info.Areas.Identity.Pages.Account.Manage
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ProfileModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Numer telefonu")]
            [StringLength(9, ErrorMessage = "Wprowadź tylko 9 cyfr bez zbędnych znaków i odstępów.")]
            [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "W numerze telefonu mogą występować wyłącznie cyfry.")]
            public string PhoneNumber { get; set; }

            // imię użytkownika
            [Required(ErrorMessage = "Należy podać imię użytkownika.")]
            [Display(Name = "Imię użytkownika")]
            [MaxLength(20, ErrorMessage = "Imię użytkownika nie może być dłuższe niż {1} znaków.")]
            public string FirstName { get; set; }

            // nazwisko użytkownika
            [Display(Name = "Nazwisko użytkownika")]
            [MaxLength(50, ErrorMessage = "Nazwisko użytkownika nie może być dłuższe niż {1} znaków.")]
            public string LastName { get; set; }

            // informacje o użytkowniku
            [Display(Name = "Informacja o użytkowniku")]
            [MaxLength(255, ErrorMessage = "Informacja o użytkowniku nie może być dłuższa niż {1} znaków.")]
            public string Information { get; set; }

            // fotografia użytkownika
            [Display(Name = "Awatar lub fotografia użytkownika")]
            [MaxLength(128, ErrorMessage = "Nazwa pliku nie może być dłuższa niż {1} znaków.")]
            public string Photo { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Information = user.Information,
                Photo = user.Photo
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

            //dodać wczytywanie i skalowanie obrazka
            //analogicznie, jak przy tekstach
            //lub zapamietanie wybranego awatara

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
            }

            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
            }

            if (Input.Information != user.Information)
            {
                user.Information = Input.Information;
            }

            //dodać kod sprawdzający i weryfikujący dane
            //przed zapisem do bazy

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
