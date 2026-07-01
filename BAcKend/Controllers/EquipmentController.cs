using Microsoft.AspNetCore.Mvc;
using GymManagement.Api.Data;
using GymManagement.Api.Models;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly EquipmentData _data;
    public EquipmentController(EquipmentData data) => _data = data;

    [HttpGet]
    public IActionResult GetAll(string? search = null, string? status = null)
    {
        List<Equipment> equipment = _data.GetAllEquipment();
        if (!string.IsNullOrWhiteSpace(search))
            equipment = equipment.Where(e => e.EquipmentName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        if (!string.IsNullOrWhiteSpace(status))
            equipment = equipment.Where(e => string.Equals(e.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();
        return Ok(equipment);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        Equipment? equipment = _data.GetEquipmentById(id);
        return equipment == null ? NotFound(new { message = "Equipment not found." }) : Ok(equipment);
    }

    [HttpPost]
    public IActionResult Create(Equipment equipment)
    {
        equipment.EquipmentName = equipment.EquipmentName.Trim();
        if (equipment.Quantity == 0) equipment.Status = "Out of Stock";
        int id = _data.AddEquipment(equipment);
        equipment.EquipmentID = id;
        return CreatedAtAction(nameof(GetById), new { id }, equipment);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Equipment equipment)
    {
        if (_data.GetEquipmentById(id) == null)
            return NotFound(new { message = "Equipment not found." });

        equipment.EquipmentID = id;
        equipment.EquipmentName = equipment.EquipmentName.Trim();
        if (equipment.Quantity == 0) equipment.Status = "Out of Stock";
        _data.UpdateEquipment(equipment);
        return Ok(new { message = "Equipment updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return _data.DeleteEquipment(id)
            ? Ok(new { message = "Equipment deleted successfully." })
            : NotFound(new { message = "Equipment not found." });
    }
}
