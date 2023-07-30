using Forest.Models;

namespace Forest.Areas.Admin.ViewModels
{
    public class UserRoleVM
    {
        public User User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
