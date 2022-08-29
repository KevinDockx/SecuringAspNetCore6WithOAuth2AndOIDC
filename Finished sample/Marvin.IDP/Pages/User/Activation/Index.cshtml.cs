using Marvin.IDP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marvin.IDP.Pages.User.Activation
{
    [SecurityHeaders]
    [AllowAnonymous]

    public class IndexModel : PageModel
    {
        private readonly ILocalUserService _localUserService;

        public IndexModel(ILocalUserService localUserService)
        {
            _localUserService = localUserService ??
                throw new ArgumentNullException(nameof(localUserService));
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGet(string securityCode)
        {
            Input = new InputModel();

            if (await _localUserService.ActivateUserAsync(securityCode))
            {
                Input.Message = "Your account was successfully activated.  " +
                    "Navigate to your client application to log in.";
            }
            else
            {
                Input.Message = "Your account couldn't be activated, " +
                    "please contact your administrator.";
            }

            await _localUserService.SaveChangesAsync();

            return Page();


        }
    }
}
