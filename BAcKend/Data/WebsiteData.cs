using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class WebsiteData
{
    private readonly DashboardData _dashboardData;
    private readonly MembershipPlanData _planData;
    private readonly TrainerData _trainerData;
    private readonly WorkoutProgramData _programData;

    public WebsiteData(
        DashboardData dashboardData,
        MembershipPlanData planData,
        TrainerData trainerData,
        WorkoutProgramData programData)
    {
        _dashboardData = dashboardData;
        _planData = planData;
        _trainerData = trainerData;
        _programData = programData;
    }

    public WebsiteHomeResponse GetHomePage()
    {
        return new WebsiteHomeResponse
        {
            HeroTitle = "Build Strength. Improve Health. Reach Your Goals.",
            HeroSubtitle = "A complete gym website and management platform for members, trainers, workout programs, equipment and payments.",
            Statistics = _dashboardData.GetSummary(),
            FeaturedPlans = _planData.GetAllPlans().Take(3).ToList(),
            FeaturedTrainers = _trainerData.GetAllTrainers().Take(3).ToList(),
            FeaturedPrograms = _programData.GetAllPrograms().Take(3).ToList()
        };
    }

    public WebsiteAboutResponse GetAboutPage()
    {
        return new WebsiteAboutResponse
        {
            Title = "About Our Gym",
            Description = "We provide a clean, organized and supportive fitness environment for beginners and experienced members.",
            Mission = "To help every member improve strength, fitness and confidence through professional training and reliable gym services.",
            Vision = "To become a trusted modern fitness center supported by simple technology and excellent customer service.",
            Services = new List<string>
            {
                "Membership plans",
                "Professional trainers",
                "Personal workout programs",
                "Modern gym equipment",
                "Member payment management"
            },
            Statistics = _dashboardData.GetSummary()
        };
    }
}
