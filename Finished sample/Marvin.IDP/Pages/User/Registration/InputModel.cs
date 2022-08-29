using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Marvin.IDP.Pages.User.Registration
{
    public class InputModel
    {
        public string ReturnUrl { get; set; }

        [MaxLength(200)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [MaxLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Given name")]
        public string GivenName { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Family name")]
        public string FamilyName { get; set; }

        [Required]
        [MaxLength(2)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public SelectList CountryCodes { get; set; } =
            new (  new[] {
                    new { Id = "be", Value = "Belgium" },
                    new { Id = "us", Value = "United States of America" },
                    new { Id = "in", Value = "India" } },
                "Id",
                "Value");
    }


}
