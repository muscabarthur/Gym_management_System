using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GymManagement.Api.Data;
using GymManagement.Api.Models;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly MemberData _data;
    public MembersController(MemberData data) => _data = data;

    [HttpGet]
    public IActionResult GetAll(string? search = null, string? gender = null)
    {
        List<Member> members = _data.GetAllMembers();

        // LINQ method syntax used for React search/filter controls.
        if (!string.IsNullOrWhiteSpace(search))
        {
            members = members.Where(m =>
                    m.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (m.Phone?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (m.Address?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(gender))
        {
            members = members.Where(m =>
                    string.Equals(m.Gender, gender, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Ok(members);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        Member? member = _data.GetMemberById(id);
        return member == null ? NotFound(new { message = "Member not found." }) : Ok(member);
    }

    [HttpPost]
    public IActionResult Create(Member member)
    {
        member.FullName = member.FullName.Trim();
        member.Phone = member.Phone?.Trim();

        if (!string.IsNullOrWhiteSpace(member.Phone) && _data.PhoneExists(member.Phone))
            return Conflict(new { message = "This member phone number already exists." });

        int id = _data.AddMember(member);
        member.MemberID = id;
        return CreatedAtAction(nameof(GetById), new { id }, member);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Member member)
    {
        if (_data.GetMemberById(id) == null)
            return NotFound(new { message = "Member not found." });

        member.FullName = member.FullName.Trim();
        member.Phone = member.Phone?.Trim();

        if (!string.IsNullOrWhiteSpace(member.Phone) && _data.PhoneExists(member.Phone, id))
            return Conflict(new { message = "This member phone number already exists." });

        member.MemberID = id;
        _data.UpdateMember(member);
        return Ok(new { message = "Member updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            return _data.DeleteMember(id)
                ? Ok(new { message = "Member deleted successfully." })
                : NotFound(new { message = "Member not found." });
        }
        catch (SqlException)
        {
            return BadRequest(new { message = "Cannot delete this member because payments or workout programs are linked to the member." });
        }
    }
}
