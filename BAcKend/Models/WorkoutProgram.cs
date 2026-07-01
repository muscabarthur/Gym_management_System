using System.ComponentModel.DataAnnotations;

namespace GymManagement.Api.Models;

public class WorkoutProgram
{
    public int ProgramID { get; set; }

    [Required, StringLength(100)]
    public string ProgramName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TrainerID { get; set; }
}

public class WorkoutProgramDetail : WorkoutProgram
{
    public string TrainerName { get; set; } = string.Empty;
    public string? Specialty { get; set; }
}
