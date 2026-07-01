using System.ComponentModel.DataAnnotations;

namespace GymManagement.Api.Models;

public class Member
{
    public int MemberID { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }
}
