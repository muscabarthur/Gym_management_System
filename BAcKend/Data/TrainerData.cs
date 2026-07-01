using Microsoft.Data.SqlClient;
using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class TrainerData
{
    private readonly string _connectionString;

    public TrainerData(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");
    }

    public List<Trainer> GetAllTrainers()
    {
        List<Trainer> trainers = new();
        const string sql = "SELECT TrainerID, TrainerName, Specialty, Phone FROM Trainers ORDER BY TrainerID DESC";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) trainers.Add(MapTrainer(reader));
        return trainers;
    }

    public Trainer? GetTrainerById(int id)
    {
        const string sql = "SELECT TrainerID, TrainerName, Specialty, Phone FROM Trainers WHERE TrainerID=@TrainerID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@TrainerID", id);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapTrainer(reader) : null;
    }

    public int AddTrainer(Trainer trainer)
    {
        const string sql = @"INSERT INTO Trainers (TrainerName, Specialty, Phone)
                             VALUES (@TrainerName, @Specialty, @Phone);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, trainer);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool UpdateTrainer(Trainer trainer)
    {
        const string sql = @"UPDATE Trainers SET TrainerName=@TrainerName,
                             Specialty=@Specialty, Phone=@Phone WHERE TrainerID=@TrainerID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, trainer);
        command.Parameters.AddWithValue("@TrainerID", trainer.TrainerID);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteTrainer(int id)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("DELETE FROM Trainers WHERE TrainerID=@TrainerID", connection);
        command.Parameters.AddWithValue("@TrainerID", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private static Trainer MapTrainer(SqlDataReader reader) => new()
    {
        TrainerID = Convert.ToInt32(reader["TrainerID"]),
        TrainerName = reader["TrainerName"].ToString() ?? string.Empty,
        Specialty = reader["Specialty"] == DBNull.Value ? null : reader["Specialty"].ToString(),
        Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"].ToString()
    };

    private static void AddParameters(SqlCommand command, Trainer trainer)
    {
        command.Parameters.AddWithValue("@TrainerName", trainer.TrainerName);
        command.Parameters.AddWithValue("@Specialty", (object?)trainer.Specialty ?? DBNull.Value);
        command.Parameters.AddWithValue("@Phone", (object?)trainer.Phone ?? DBNull.Value);
    }
}
