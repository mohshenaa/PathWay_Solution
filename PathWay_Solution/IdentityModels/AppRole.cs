using Microsoft.AspNetCore.Identity;

namespace PathWay_Solution.IdentityModels
{
    public class AppRole:IdentityRole<Guid>
    {
        public string? Description {  get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
