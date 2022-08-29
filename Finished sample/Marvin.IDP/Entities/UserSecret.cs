using System.ComponentModel.DataAnnotations;

namespace Marvin.IDP.Entities
{
    public class UserSecret : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; }

        public string ConcurrencyStamp { get; set; } =
            Guid.NewGuid().ToString();
    }
}
