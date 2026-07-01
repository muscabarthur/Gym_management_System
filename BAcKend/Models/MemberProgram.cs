using System.ComponentModel.DataAnnotations;

namespace GymManagement.Api.Models;

public class MemberProgram
{
    public int MemberProgramID { get; set; }

    [Range(1, int.MaxValue)]
    public int MemberID { get; set; }

    [Range(1, int.MaxValue)]
    public int ProgramID { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Today;
}

public class MemberProgramDetail : MemberProgram
{
    public string MemberName { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string TrainerName { get; set; } = string.Empty;
}
