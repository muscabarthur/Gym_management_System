using Microsoft.AspNetCore.Mvc;
using GymManagement.Api.Data;
using GymManagement.Api.Models;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemberProgramsController : ControllerBase
{
    private readonly MemberProgramData _data;
    private readonly MemberData _memberData;
    private readonly WorkoutProgramData _programData;

    public MemberProgramsController(MemberProgramData data, MemberData memberData, WorkoutProgramData programData)
    {
        _data = data;
        _memberData = memberData;
        _programData = programData;
    }

    [HttpGet]
    public IActionResult GetAll(string? search = null, int? memberId = null, int? programId = null)
    {
        List<MemberProgramDetail> assignments = _data.GetAllMemberPrograms();
        if (!string.IsNullOrWhiteSpace(search))
        {
            assignments = assignments.Where(a =>
                    a.MemberName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.ProgramName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.TrainerName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        if (memberId.HasValue)
            assignments = assignments.Where(a => a.MemberID == memberId.Value).ToList();
        if (programId.HasValue)
            assignments = assignments.Where(a => a.ProgramID == programId.Value).ToList();
        return Ok(assignments);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        MemberProgramDetail? assignment = _data.GetMemberProgramById(id);
        return assignment == null ? NotFound(new { message = "Member program assignment not found." }) : Ok(assignment);
    }

    [HttpPost]
    public IActionResult Create(MemberProgram assignment)
    {
        string? validationMessage = ValidateForeignKeys(assignment);
        if (validationMessage != null) return BadRequest(new { message = validationMessage });

        if (_data.AssignmentExists(assignment.MemberID, assignment.ProgramID))
            return Conflict(new { message = "This member is already assigned to this workout program." });

        int id = _data.AddMemberProgram(assignment);
        assignment.MemberProgramID = id;
        return CreatedAtAction(nameof(GetById), new { id }, assignment);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, MemberProgram assignment)
    {
        if (_data.GetMemberProgramById(id) == null)
            return NotFound(new { message = "Member program assignment not found." });

        string? validationMessage = ValidateForeignKeys(assignment);
        if (validationMessage != null) return BadRequest(new { message = validationMessage });

        if (_data.AssignmentExists(assignment.MemberID, assignment.ProgramID, id))
            return Conflict(new { message = "This member is already assigned to this workout program." });

        assignment.MemberProgramID = id;
        _data.UpdateMemberProgram(assignment);
        return Ok(new { message = "Member program assignment updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return _data.DeleteMemberProgram(id)
            ? Ok(new { message = "Member program assignment deleted successfully." })
            : NotFound(new { message = "Member program assignment not found." });
    }

    private string? ValidateForeignKeys(MemberProgram assignment)
    {
        if (_memberData.GetMemberById(assignment.MemberID) == null)
            return "The selected member does not exist.";
        if (_programData.GetProgramById(assignment.ProgramID) == null)
            return "The selected workout program does not exist.";
        return null;
    }
}
