using Microsoft.Data.SqlClient;
using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class WorkoutProgramData
{
    private readonly string _connectionString;

    public WorkoutProgramData(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");
    }

    public List<WorkoutProgramDetail> GetAllPrograms()
    {
        List<WorkoutProgramDetail> programs = new();
        const string sql = @"SELECT w.ProgramID, w.ProgramName, w.TrainerID,
                                    t.TrainerName, t.Specialty
                             FROM WorkoutPrograms w
                             INNER JOIN Trainers t ON w.TrainerID=t.TrainerID
                             ORDER BY w.ProgramID DESC";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) programs.Add(MapProgramDetail(reader));
        return programs;
    }

    public WorkoutProgramDetail? GetProgramById(int id)
    {
        const string sql = @"SELECT w.ProgramID, w.ProgramName, w.TrainerID,
                                    t.TrainerName, t.Specialty
                             FROM WorkoutPrograms w
                             INNER JOIN Trainers t ON w.TrainerID=t.TrainerID
                             WHERE w.ProgramID=@ProgramID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@ProgramID", id);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapProgramDetail(reader) : null;
    }

    public int AddProgram(WorkoutProgram program)
    {
        const string sql = @"INSERT INTO WorkoutPrograms (ProgramName, TrainerID)
                             VALUES (@ProgramName, @TrainerID);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, program);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool UpdateProgram(WorkoutProgram program)
    {
        const string sql = @"UPDATE WorkoutPrograms SET ProgramName=@ProgramName,
                             TrainerID=@TrainerID WHERE ProgramID=@ProgramID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, program);
        command.Parameters.AddWithValue("@ProgramID", program.ProgramID);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteProgram(int id)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("DELETE FROM WorkoutPrograms WHERE ProgramID=@ProgramID", connection);
        command.Parameters.AddWithValue("@ProgramID", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private static WorkoutProgramDetail MapProgramDetail(SqlDataReader reader) => new()
    {
        ProgramID = Convert.ToInt32(reader["ProgramID"]),
        ProgramName = reader["ProgramName"].ToString() ?? string.Empty,
        TrainerID = Convert.ToInt32(reader["TrainerID"]),
        TrainerName = reader["TrainerName"].ToString() ?? string.Empty,
        Specialty = reader["Specialty"] == DBNull.Value ? null : reader["Specialty"].ToString()
    };

    private static void AddParameters(SqlCommand command, WorkoutProgram program)
    {
        command.Parameters.AddWithValue("@ProgramName", program.ProgramName);
        command.Parameters.AddWithValue("@TrainerID", program.TrainerID);
    }
}
