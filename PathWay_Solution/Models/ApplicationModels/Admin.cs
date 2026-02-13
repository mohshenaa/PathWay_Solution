using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models.ApplicationModels
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required, StringLength(maximumLength: 50)]
        public string AdminName { get; set; } = null!;

        [Required, StringLength(maximumLength: 30)]
        public string PhoneNumber { get; set; } = null!;

        [StringLength(100)]
        public string? Email { get; set; }
        public string ImageUrl { get; set; } = "";

        public Guid? AppUserId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
