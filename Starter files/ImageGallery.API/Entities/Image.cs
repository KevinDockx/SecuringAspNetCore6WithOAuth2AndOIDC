using System;
using System.ComponentModel.DataAnnotations;

namespace ImageGallery.API.Entities
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string OwnerId { get; set; } = string.Empty;
    }
}
