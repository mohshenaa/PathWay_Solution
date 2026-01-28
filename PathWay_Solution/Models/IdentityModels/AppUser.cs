using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace PathWay_Solution.Models.IdentityModels
{
    public class AppUser:IdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } 

        public DateTime? DateOfBirth { get; set; }
        public DateTime? LastLogin {  get; set; }
        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual List<Address>? Address { get; set; }
    }
}
