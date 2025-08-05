using HomeEstate.Services.Core.Dtos;

namespace HomeEstate.Web.Areas.Admin.Models
{
    public class AdminUsersViewModel
    {
        public Pagination<ApplicationUserWithRoleDto> Users { get; set; } = new();
        public string SearchTerm { get; set; } = "";
        public string RoleFilter { get; set; } = "";
        public int[] PageSizes { get; set; } = { 5, 10, 25, 50 };
        public List<string> AvailableRoles { get; set; } = new();
    }
}