using Microsoft.Data.SqlClient;
using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class MemberData
{
    private readonly string _connectionString;

    public MemberData(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");
    }

    public List<Member> GetAllMembers()
    {
        List<Member> members = new();
        const string sql = "SELECT MemberID, FullName, Gender, Phone, Address FROM Members ORDER BY MemberID DESC";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read()) members.Add(MapMember(reader));
        return members;
    }

    public Member? GetMemberById(int id)
    {
        const string sql = "SELECT MemberID, FullName, Gender, Phone, Address FROM Members WHERE MemberID=@MemberID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@MemberID", id);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapMember(reader) : null;
    }

    public bool PhoneExists(string phone, int? ignoreMemberId = null)
    {
        string sql = "SELECT COUNT(*) FROM Members WHERE Phone=@Phone";
        if (ignoreMemberId.HasValue) sql += " AND MemberID<>@MemberID";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Phone", phone);
        if (ignoreMemberId.HasValue) command.Parameters.AddWithValue("@MemberID", ignoreMemberId.Value);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public int AddMember(Member member)
    {
        const string sql = @"INSERT INTO Members (FullName, Gender, Phone, Address)
                             VALUES (@FullName, @Gender, @Phone, @Address);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, member);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool UpdateMember(Member member)
    {
        const string sql = @"UPDATE Members SET FullName=@FullName, Gender=@Gender,
                             Phone=@Phone, Address=@Address WHERE MemberID=@MemberID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, member);
        command.Parameters.AddWithValue("@MemberID", member.MemberID);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteMember(int id)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("DELETE FROM Members WHERE MemberID=@MemberID", connection);
        command.Parameters.AddWithValue("@MemberID", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private static Member MapMember(SqlDataReader reader) => new()
    {
        MemberID = Convert.ToInt32(reader["MemberID"]),
        FullName = reader["FullName"].ToString() ?? string.Empty,
        Gender = reader["Gender"] == DBNull.Value ? null : reader["Gender"].ToString(),
        Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"].ToString(),
        Address = reader["Address"] == DBNull.Value ? null : reader["Address"].ToString()
    };

    private static void AddParameters(SqlCommand command, Member member)
    {
        command.Parameters.AddWithValue("@FullName", member.FullName);
        command.Parameters.AddWithValue("@Gender", (object?)member.Gender ?? DBNull.Value);
        command.Parameters.AddWithValue("@Phone", (object?)member.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Address", (object?)member.Address ?? DBNull.Value);
    }
}
