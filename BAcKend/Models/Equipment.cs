using System.ComponentModel.DataAnnotations;

namespace GymManagement.Api.Models;

public class Equipment
{
    public int EquipmentID { get; set; }

    [Required, StringLength(100)]
    public string EquipmentName { get; set; } = string.Empty;

    [Range(0, 100000)]
    public int Quantity { get; set; }

    [Required, StringLength(20)]
    public string Status { get; set; } = "Available";
}
