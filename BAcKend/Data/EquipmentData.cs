using Microsoft.Data.SqlClient;
using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class EquipmentData
{
    private readonly string _connectionString;

    public EquipmentData(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");
    }

    public List<Equipment> GetAllEquipment()
    {
        List<Equipment> equipment = new();
        const string sql = "SELECT EquipmentID, EquipmentName, Quantity, Status FROM Equipment ORDER BY EquipmentID DESC";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) equipment.Add(MapEquipment(reader));
        return equipment;
    }

    public Equipment? GetEquipmentById(int id)
    {
        const string sql = "SELECT EquipmentID, EquipmentName, Quantity, Status FROM Equipment WHERE EquipmentID=@EquipmentID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@EquipmentID", id);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapEquipment(reader) : null;
    }

    public int AddEquipment(Equipment equipment)
    {
        const string sql = @"INSERT INTO Equipment (EquipmentName, Quantity, Status)
                             VALUES (@EquipmentName, @Quantity, @Status);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, equipment);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool UpdateEquipment(Equipment equipment)
    {
        const string sql = @"UPDATE Equipment SET EquipmentName=@EquipmentName,
                             Quantity=@Quantity, Status=@Status WHERE EquipmentID=@EquipmentID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, equipment);
        command.Parameters.AddWithValue("@EquipmentID", equipment.EquipmentID);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteEquipment(int id)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("DELETE FROM Equipment WHERE EquipmentID=@EquipmentID", connection);
        command.Parameters.AddWithValue("@EquipmentID", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private static Equipment MapEquipment(SqlDataReader reader) => new()
    {
        EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
        EquipmentName = reader["EquipmentName"].ToString() ?? string.Empty,
        Quantity = reader["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Quantity"]),
        Status = reader["Status"].ToString() ?? "Available"
    };

    private static void AddParameters(SqlCommand command, Equipment equipment)
    {
        command.Parameters.AddWithValue("@EquipmentName", equipment.EquipmentName);
        command.Parameters.AddWithValue("@Quantity", equipment.Quantity);
        command.Parameters.AddWithValue("@Status", equipment.Status);
    }
}
