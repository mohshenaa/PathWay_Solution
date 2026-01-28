using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models.IdentityModels
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        [ForeignKey(nameof(UserId))]
        public Guid UserId { get; set; }

        public virtual AppUser AppUser { get; set; } = default!;

        public string? Street { get; set; } 
        public string City { get; set; } = default!;
        public string? State { get; set; }
        public string? PostalCode { get; set; } 
        public string Country { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}