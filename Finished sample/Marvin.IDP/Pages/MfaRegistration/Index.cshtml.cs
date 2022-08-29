using IdentityModel;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Marvin.IDP.Pages.MfaRegistration
{
    [SecurityHeaders]
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILocalUserService _localUserService;
        private readonly char[] chars =
           "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public ViewModel View { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IndexModel(ILocalUserService localUserService)
        {
            _localUserService = localUserService ??
                throw new ArgumentNullException(nameof(localUserService));
        }

        public async Task OnGet()
        {
            var tokenData = RandomNumberGenerator.GetBytes(64);

            var result = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
            {
                var rnd = BitConverter.ToUInt32(tokenData, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            var secret = result.ToString();

            var subject = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            var user = await _localUserService.GetUserBySubjectAsync(subject);

            var keyUri = string.Format(
               "otpauth://totp/{0}:{1}?secret={2}&issuer={0}",
               WebUtility.UrlEncode("Marvin"),
               WebUtility.UrlEncode(user.Email),
               secret);

            View = new ()
            {
                KeyUri = keyUri
            };

            Input = new ()
            {
                Secret = secret
            }; 
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var subject = User.FindFirst(JwtClaimTypes.Subject)?.Value;
                if (await _localUserService
                    .AddUserSecret(subject, "TOTP", Input.Secret))
                {
                    await _localUserService.SaveChangesAsync();
                    // return to the root 
                    return Redirect("~/");
                }
                else
                {
                    throw new Exception("MFA registration error");
                }
            }
            else
            {
                return Page();
            }
        }

    }
}
