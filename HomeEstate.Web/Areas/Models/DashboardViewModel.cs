using HomeEstate.Web.Models;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalProperties { get; set; }
    public int TotalAdmins { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewPropertiesThisMonth { get; set; }
    public List<ApplicationUserViewModel> RecentUsers { get; set; } = new List<ApplicationUserViewModel>();
    public List<PropertyViewModel> RecentProperties { get; set; } = new List<PropertyViewModel>();
    public Dictionary<string, int> UserRegistrationStats { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> PropertyCreationStats { get; set; } = new Dictionary<string, int>();
}