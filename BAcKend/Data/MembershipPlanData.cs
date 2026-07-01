using Microsoft.Data.SqlClient;
using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class MembershipPlanData
{
    private readonly string _connectionString;

    public MembershipPlanData(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");
    }

    public List<MembershipPlan> GetAllPlans()
    {
        List<MembershipPlan> plans = new();
        const string sql = "SELECT PlanID, PlanName, DurationMonths, Price FROM MembershipPlans ORDER BY Price";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) plans.Add(MapPlan(reader));
        return plans;
    }

    public MembershipPlan? GetPlanById(int id)
    {
        const string sql = "SELECT PlanID, PlanName, DurationMonths, Price FROM MembershipPlans WHERE PlanID=@PlanID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@PlanID", id);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapPlan(reader) : null;
    }

    public int AddPlan(MembershipPlan plan)
    {
        const string sql = @"INSERT INTO MembershipPlans (PlanName, DurationMonths, Price)
                             VALUES (@PlanName, @DurationMonths, @Price);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, plan);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool UpdatePlan(MembershipPlan plan)
    {
        const string sql = @"UPDATE MembershipPlans SET PlanName=@PlanName,
                             DurationMonths=@DurationMonths, Price=@Price WHERE PlanID=@PlanID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, plan);
        command.Parameters.AddWithValue("@PlanID", plan.PlanID);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeletePlan(int id)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("DELETE FROM MembershipPlans WHERE PlanID=@PlanID", connection);
        command.Parameters.AddWithValue("@PlanID", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private static MembershipPlan MapPlan(SqlDataReader reader) => new()
    {
        PlanID = Convert.ToInt32(reader["PlanID"]),
        PlanName = reader["PlanName"].ToString() ?? string.Empty,
        DurationMonths = reader["DurationMonths"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DurationMonths"]),
        Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"])
    };

    private static void AddParameters(SqlCommand command, MembershipPlan plan)
    {
        command.Parameters.AddWithValue("@PlanName", plan.PlanName);
        command.Parameters.AddWithValue("@DurationMonths", plan.DurationMonths);
        command.Parameters.AddWithValue("@Price", plan.Price);
    }
}
