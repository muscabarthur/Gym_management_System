using System.ComponentModel.DataAnnotations;

namespace GymManagement.Api.Models;

public class Trainer
{
    public int TrainerID { get; set; }

    [Required, StringLength(100)]
    public string TrainerName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Specialty { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }
}
