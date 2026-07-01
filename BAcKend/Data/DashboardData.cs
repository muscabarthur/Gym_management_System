using Microsoft.Data.SqlClient;
using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class DashboardData
{
    private readonly string _connectionString;
    private readonly MemberData _memberData;
    private readonly PaymentData _paymentData;

    public DashboardData(IConfiguration configuration, MemberData memberData, PaymentData paymentData)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");
        _memberData = memberData;
        _paymentData = paymentData;
    }

    public DashboardSummary GetSummary()
    {
        return new DashboardSummary
        {
            TotalMembers = GetCount("Members"),
            TotalTrainers = GetCount("Trainers"),
            TotalPlans = GetCount("MembershipPlans"),
            TotalPrograms = GetCount("WorkoutPrograms"),
            TotalEquipmentItems = GetEquipmentQuantity(),
            ActiveProgramEnrollments = GetCount("MemberPrograms"),
            TotalPayments = _paymentData.GetTotalPayments(),
            ThisMonthPayments = _paymentData.GetThisMonthPayments(),
            RecentPayments = _paymentData.GetAllPayments().Take(5).ToList(),
            RecentMembers = _memberData.GetAllMembers().Take(5).ToList()
        };
    }

    private int GetCount(string tableName)
    {
        // tableName comes only from hard-coded names above, not from user input.
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new($"SELECT COUNT(*) FROM {tableName}", connection);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private int GetEquipmentQuantity()
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("SELECT ISNULL(SUM(Quantity), 0) FROM Equipment", connection);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }
}
