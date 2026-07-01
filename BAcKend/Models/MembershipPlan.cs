using System.ComponentModel.DataAnnotations;

namespace GymManagement.Api.Models;

public class MembershipPlan
{
    public int PlanID { get; set; }

    [Required, StringLength(50)]
    public string PlanName { get; set; } = string.Empty;

    [Range(1, 120)]
    public int DurationMonths { get; set; }

    [Range(0, 1000000)]
    public decimal Price { get; set; }
}
