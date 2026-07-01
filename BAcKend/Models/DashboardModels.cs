namespace GymManagement.Api.Models;

public class DashboardSummary
{
    public int TotalMembers { get; set; }
    public int TotalTrainers { get; set; }
    public int TotalPlans { get; set; }
    public int TotalPrograms { get; set; }
    public int TotalEquipmentItems { get; set; }
    public int ActiveProgramEnrollments { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal ThisMonthPayments { get; set; }
    public List<PaymentDetail> RecentPayments { get; set; } = new();
    public List<Member> RecentMembers { get; set; } = new();
}

public class WebsiteHomeResponse
{
    public string HeroTitle { get; set; } = string.Empty;
    public string HeroSubtitle { get; set; } = string.Empty;
    public DashboardSummary Statistics { get; set; } = new();
    public List<MembershipPlan> FeaturedPlans { get; set; } = new();
    public List<Trainer> FeaturedTrainers { get; set; } = new();
    public List<WorkoutProgramDetail> FeaturedPrograms { get; set; } = new();
}

public class WebsiteAboutResponse
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Mission { get; set; } = string.Empty;
    public string Vision { get; set; } = string.Empty;
    public List<string> Services { get; set; } = new();
    public DashboardSummary Statistics { get; set; } = new();
}

public class MemberPaymentReport
{
    public int MemberID { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int PaymentCount { get; set; }
    public decimal TotalPaid { get; set; }
}

public class EquipmentStatusReport
{
    public string Status { get; set; } = string.Empty;
    public int EquipmentTypes { get; set; }
    public int TotalQuantity { get; set; }
}

public class ProgramEnrollmentReport
{
    public int ProgramID { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int MembersCount { get; set; }
}
