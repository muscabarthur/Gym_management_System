using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GymManagement.Api.Data;
using GymManagement.Api.Models;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainersController : ControllerBase
{
    private readonly TrainerData _data;
    public TrainersController(TrainerData data) => _data = data;

    [HttpGet]
    public IActionResult GetAll(string? search = null)
    {
        List<Trainer> trainers = _data.GetAllTrainers();
        if (!string.IsNullOrWhiteSpace(search))
        {
            trainers = trainers.Where(t =>
                    t.TrainerName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (t.Specialty?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (t.Phone?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }
        return Ok(trainers);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        Trainer? trainer = _data.GetTrainerById(id);
        return trainer == null ? NotFound(new { message = "Trainer not found." }) : Ok(trainer);
    }

    [HttpPost]
    public IActionResult Create(Trainer trainer)
    {
        trainer.TrainerName = trainer.TrainerName.Trim();
        int id = _data.AddTrainer(trainer);
        trainer.TrainerID = id;
        return CreatedAtAction(nameof(GetById), new { id }, trainer);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Trainer trainer)
    {
        if (_data.GetTrainerById(id) == null)
            return NotFound(new { message = "Trainer not found." });

        trainer.TrainerID = id;
        trainer.TrainerName = trainer.TrainerName.Trim();
        _data.UpdateTrainer(trainer);
        return Ok(new { message = "Trainer updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            return _data.DeleteTrainer(id)
                ? Ok(new { message = "Trainer deleted successfully." })
                : NotFound(new { message = "Trainer not found." });
        }
        catch (SqlException)
        {
            return BadRequest(new { message = "Cannot delete this trainer because workout programs are linked to the trainer." });
        }
    }
}
