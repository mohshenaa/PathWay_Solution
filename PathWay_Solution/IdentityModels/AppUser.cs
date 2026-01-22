using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace PathWay_Solution.IdentityModels
{
    public class AppUser:IdentityUser<Guid>
    {
        public string FastName { get; set; } = null!;
        public string? LastName { get; set; } 

        public DateTime? DateOfBirth { get; set; }
        public DateTime? LastLogin {  get; set; }
        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual List<Address>? Addresses { get; set; }
    }
}
