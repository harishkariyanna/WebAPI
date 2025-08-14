using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProjectTrackerAPI.Data;
using EmployeeProjectTrackerAPI.Models;

namespace EmployeeProjectTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<ActionResult> GetProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.Employees)
                .Select(p => new
                {
                    p.ProjectId,
                    p.ProjectCode,
                    p.ProjectName,
                    p.StartDate,
                    p.EndDate,
                    p.Budget,
                    Employees = p.Employees.Select(e => new
                    {
                        e.EmployeeId,
                        e.EmployeeCode,
                        e.FullName,
                        e.Email,
                        e.Designation,
                        e.Salary
                    })
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET: api/projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            var response = new
            {
                project.ProjectId,
                project.ProjectCode,
                project.ProjectName,
                project.StartDate,
                project.EndDate,
                project.Budget,
                Employees = project.Employees.Select(e => new
                {
                    e.EmployeeId,
                    e.EmployeeCode,
                    e.FullName,
                    e.Email,
                    e.Designation,
                    e.Salary
                })
            };

            return Ok(response);
        }

        // POST: api/projects
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            if (await _context.Projects.AnyAsync(p => p.ProjectCode == project.ProjectCode))
            {
                return BadRequest($"Project with code '{project.ProjectCode}' already exists.");
            }

            project.ProjectId = 0;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
        }

        // PUT: api/projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest("Project ID mismatch.");
            }

            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            if (await _context.Projects.AnyAsync(p => p.ProjectCode == project.ProjectCode && p.ProjectId != id))
            {
                return BadRequest($"Project with code '{project.ProjectCode}' already exists.");
            }

            existingProject.ProjectCode = project.ProjectCode;
            existingProject.ProjectName = project.ProjectName;
            existingProject.StartDate = project.StartDate;
            existingProject.EndDate = project.EndDate;
            existingProject.Budget = project.Budget;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            if (project.Employees.Any())
            {
                return BadRequest("Cannot delete project with assigned employees. Please reassign or remove employees first.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
