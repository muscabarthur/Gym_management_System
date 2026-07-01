using System.ComponentModel.DataAnnotations;

namespace GymManagement.Api.Models;

public class Payment
{
    public int PaymentID { get; set; }

    [Range(1, int.MaxValue)]
    public int MemberID { get; set; }

    [Range(0.01, 1000000)]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Today;
}

public class PaymentDetail : Payment
{
    public string MemberName { get; set; } = string.Empty;
}
