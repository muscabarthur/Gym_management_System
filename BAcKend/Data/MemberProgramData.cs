using Microsoft.Data.SqlClient;
using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class MemberProgramData
{
    private readonly string _connectionString;

    public MemberProgramData(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");
    }

    public List<MemberProgramDetail> GetAllMemberPrograms()
    {
        List<MemberProgramDetail> assignments = new();
        const string sql = @"SELECT mp.MemberProgramID, mp.MemberID, mp.ProgramID, mp.StartDate,
                                    m.FullName AS MemberName, w.ProgramName, t.TrainerName
                             FROM MemberPrograms mp
                             INNER JOIN Members m ON mp.MemberID=m.MemberID
                             INNER JOIN WorkoutPrograms w ON mp.ProgramID=w.ProgramID
                             INNER JOIN Trainers t ON w.TrainerID=t.TrainerID
                             ORDER BY mp.StartDate DESC, mp.MemberProgramID DESC";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) assignments.Add(MapMemberProgramDetail(reader));
        return assignments;
    }

    public MemberProgramDetail? GetMemberProgramById(int id)
    {
        const string sql = @"SELECT mp.MemberProgramID, mp.MemberID, mp.ProgramID, mp.StartDate,
                                    m.FullName AS MemberName, w.ProgramName, t.TrainerName
                             FROM MemberPrograms mp
                             INNER JOIN Members m ON mp.MemberID=m.MemberID
                             INNER JOIN WorkoutPrograms w ON mp.ProgramID=w.ProgramID
                             INNER JOIN Trainers t ON w.TrainerID=t.TrainerID
                             WHERE mp.MemberProgramID=@MemberProgramID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@MemberProgramID", id);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapMemberProgramDetail(reader) : null;
    }

    public bool AssignmentExists(int memberId, int programId, int? ignoreId = null)
    {
        string sql = "SELECT COUNT(*) FROM MemberPrograms WHERE MemberID=@MemberID AND ProgramID=@ProgramID";
        if (ignoreId.HasValue) sql += " AND MemberProgramID<>@MemberProgramID";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@MemberID", memberId);
        command.Parameters.AddWithValue("@ProgramID", programId);
        if (ignoreId.HasValue) command.Parameters.AddWithValue("@MemberProgramID", ignoreId.Value);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public int AddMemberProgram(MemberProgram assignment)
    {
        const string sql = @"INSERT INTO MemberPrograms (MemberID, ProgramID, StartDate)
                             VALUES (@MemberID, @ProgramID, @StartDate);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, assignment);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool UpdateMemberProgram(MemberProgram assignment)
    {
        const string sql = @"UPDATE MemberPrograms SET MemberID=@MemberID,
                             ProgramID=@ProgramID, StartDate=@StartDate
                             WHERE MemberProgramID=@MemberProgramID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, assignment);
        command.Parameters.AddWithValue("@MemberProgramID", assignment.MemberProgramID);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteMemberProgram(int id)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("DELETE FROM MemberPrograms WHERE MemberProgramID=@MemberProgramID", connection);
        command.Parameters.AddWithValue("@MemberProgramID", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private static MemberProgramDetail MapMemberProgramDetail(SqlDataReader reader) => new()
    {
        MemberProgramID = Convert.ToInt32(reader["MemberProgramID"]),
        MemberID = Convert.ToInt32(reader["MemberID"]),
        ProgramID = Convert.ToInt32(reader["ProgramID"]),
        StartDate = Convert.ToDateTime(reader["StartDate"]),
        MemberName = reader["MemberName"].ToString() ?? string.Empty,
        ProgramName = reader["ProgramName"].ToString() ?? string.Empty,
        TrainerName = reader["TrainerName"].ToString() ?? string.Empty
    };

    private static void AddParameters(SqlCommand command, MemberProgram assignment)
    {
        command.Parameters.AddWithValue("@MemberID", assignment.MemberID);
        command.Parameters.AddWithValue("@ProgramID", assignment.ProgramID);
        command.Parameters.AddWithValue("@StartDate", assignment.StartDate.Date);
    }
}
