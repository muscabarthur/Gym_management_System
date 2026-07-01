using Microsoft.Data.SqlClient;
using GymManagement.Api.Models;

namespace GymManagement.Api.Data;

public class PaymentData
{
    private readonly string _connectionString;

    public PaymentData(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");
    }

    public List<PaymentDetail> GetAllPayments()
    {
        List<PaymentDetail> payments = new();
        const string sql = @"SELECT p.PaymentID, p.MemberID, m.FullName AS MemberName,
                                    p.Amount, p.PaymentDate
                             FROM Payments p
                             INNER JOIN Members m ON p.MemberID=m.MemberID
                             ORDER BY p.PaymentDate DESC, p.PaymentID DESC";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) payments.Add(MapPaymentDetail(reader));
        return payments;
    }

    public PaymentDetail? GetPaymentById(int id)
    {
        const string sql = @"SELECT p.PaymentID, p.MemberID, m.FullName AS MemberName,
                                    p.Amount, p.PaymentDate
                             FROM Payments p
                             INNER JOIN Members m ON p.MemberID=m.MemberID
                             WHERE p.PaymentID=@PaymentID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@PaymentID", id);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapPaymentDetail(reader) : null;
    }

    public int AddPayment(Payment payment)
    {
        const string sql = @"INSERT INTO Payments (MemberID, Amount, PaymentDate)
                             VALUES (@MemberID, @Amount, @PaymentDate);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, payment);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool UpdatePayment(Payment payment)
    {
        const string sql = @"UPDATE Payments SET MemberID=@MemberID, Amount=@Amount,
                             PaymentDate=@PaymentDate WHERE PaymentID=@PaymentID";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        AddParameters(command, payment);
        command.Parameters.AddWithValue("@PaymentID", payment.PaymentID);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeletePayment(int id)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("DELETE FROM Payments WHERE PaymentID=@PaymentID", connection);
        command.Parameters.AddWithValue("@PaymentID", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public decimal GetTotalPayments()
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new("SELECT ISNULL(SUM(Amount), 0) FROM Payments", connection);
        connection.Open();
        return Convert.ToDecimal(command.ExecuteScalar());
    }

    public decimal GetThisMonthPayments()
    {
        const string sql = @"SELECT ISNULL(SUM(Amount), 0) FROM Payments
                             WHERE YEAR(PaymentDate)=YEAR(GETDATE())
                             AND MONTH(PaymentDate)=MONTH(GETDATE())";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);
        connection.Open();
        return Convert.ToDecimal(command.ExecuteScalar());
    }

    private static PaymentDetail MapPaymentDetail(SqlDataReader reader) => new()
    {
        PaymentID = Convert.ToInt32(reader["PaymentID"]),
        MemberID = Convert.ToInt32(reader["MemberID"]),
        MemberName = reader["MemberName"].ToString() ?? string.Empty,
        Amount = Convert.ToDecimal(reader["Amount"]),
        PaymentDate = Convert.ToDateTime(reader["PaymentDate"])
    };

    private static void AddParameters(SqlCommand command, Payment payment)
    {
        command.Parameters.AddWithValue("@MemberID", payment.MemberID);
        command.Parameters.AddWithValue("@Amount", payment.Amount);
        command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate.Date);
    }
}
