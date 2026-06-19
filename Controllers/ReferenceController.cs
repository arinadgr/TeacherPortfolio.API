using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherPortfolio.API.Models;

namespace TeacherPortfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReferenceController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReferenceController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("academicyears")]
    public async Task<IActionResult> GetAcademicYears()
    {
        var items = await _context.Academicyears
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("eventlevels")]
    public async Task<IActionResult> GetEventLevels()
    {
        var items = await _context.Eventlevels
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("participationresults")]
    public async Task<IActionResult> GetParticipationResults()
    {
        var items = await _context.Participationresults
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("methodicalmaterialtypes")]
    public async Task<IActionResult> GetMethodicalMaterialTypes()
    {
        var items = await _context.Methodicalmaterialtypes
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();
        return Ok(items);
    }
}