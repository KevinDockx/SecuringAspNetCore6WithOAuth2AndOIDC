using System.ComponentModel.DataAnnotations;

namespace Marvin.IDP.Entities
{
    public class UserLogin : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Provider { get; set; }

        [MaxLength(200)]
        [Required]
        public string ProviderIdentityKey { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
