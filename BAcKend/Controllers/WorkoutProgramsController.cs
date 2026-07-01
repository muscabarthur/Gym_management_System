using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GymManagement.Api.Data;
using GymManagement.Api.Models;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutProgramsController : ControllerBase
{
    private readonly WorkoutProgramData _data;
    private readonly TrainerData _trainerData;

    public WorkoutProgramsController(WorkoutProgramData data, TrainerData trainerData)
    {
        _data = data;
        _trainerData = trainerData;
    }

    [HttpGet]
    public IActionResult GetAll(string? search = null, int? trainerId = null)
    {
        List<WorkoutProgramDetail> programs = _data.GetAllPrograms();
        if (!string.IsNullOrWhiteSpace(search))
        {
            programs = programs.Where(p =>
                    p.ProgramName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.TrainerName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        if (trainerId.HasValue)
            programs = programs.Where(p => p.TrainerID == trainerId.Value).ToList();
        return Ok(programs);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        WorkoutProgramDetail? program = _data.GetProgramById(id);
        return program == null ? NotFound(new { message = "Workout program not found." }) : Ok(program);
    }

    [HttpPost]
    public IActionResult Create(WorkoutProgram program)
    {
        if (_trainerData.GetTrainerById(program.TrainerID) == null)
            return BadRequest(new { message = "The selected trainer does not exist." });

        program.ProgramName = program.ProgramName.Trim();
        int id = _data.AddProgram(program);
        program.ProgramID = id;
        return CreatedAtAction(nameof(GetById), new { id }, program);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, WorkoutProgram program)
    {
        if (_data.GetProgramById(id) == null)
            return NotFound(new { message = "Workout program not found." });
        if (_trainerData.GetTrainerById(program.TrainerID) == null)
            return BadRequest(new { message = "The selected trainer does not exist." });

        program.ProgramID = id;
        program.ProgramName = program.ProgramName.Trim();
        _data.UpdateProgram(program);
        return Ok(new { message = "Workout program updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            return _data.DeleteProgram(id)
                ? Ok(new { message = "Workout program deleted successfully." })
                : NotFound(new { message = "Workout program not found." });
        }
        catch (SqlException)
        {
            return BadRequest(new { message = "Cannot delete this program because members are assigned to it." });
        }
    }
}
