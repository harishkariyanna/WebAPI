using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProjectTrackerAPI.Data;
using EmployeeProjectTrackerAPI.Models;

namespace EmployeeProjectTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Project)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.EmployeeCode,
                    e.FullName,
                    e.Email,
                    e.Designation,
                    e.Salary,
                    e.ProjectId,
                    Project = new
                    {
                        e.Project.ProjectId,
                        e.Project.ProjectCode,
                        e.Project.ProjectName,
                        e.Project.StartDate,
                        e.Project.EndDate,
                        e.Project.Budget
                    }
                })
                .ToListAsync();

            return Ok(employees);
        }

        // GET: api/employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Project)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            var response = new
            {
                employee.EmployeeId,
                employee.EmployeeCode,
                employee.FullName,
                employee.Email,
                employee.Designation,
                employee.Salary,
                employee.ProjectId,
                Project = new
                {
                    employee.Project.ProjectId,
                    employee.Project.ProjectCode,
                    employee.Project.ProjectName,
                    employee.Project.StartDate,
                    employee.Project.EndDate,
                    employee.Project.Budget
                }
            };

            return Ok(response);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == employee.ProjectId);
            if (!projectExists)
            {
                return BadRequest($"Project with ID {employee.ProjectId} does not exist.");
            }

            if (await _context.Employees.AnyAsync(e => e.EmployeeCode == employee.EmployeeCode))
            {
                return BadRequest($"Employee with code '{employee.EmployeeCode}' already exists.");
            }

            if (await _context.Employees.AnyAsync(e => e.Email == employee.Email))
            {
                return BadRequest($"Employee with email '{employee.Email}' already exists.");
            }

            employee.EmployeeId = 0;
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
        }

        // PUT: api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest("Employee ID mismatch.");
            }

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == employee.ProjectId);
            if (!projectExists)
            {
                return BadRequest($"Project with ID {employee.ProjectId} does not exist.");
            }

            if (await _context.Employees.AnyAsync(e => e.EmployeeCode == employee.EmployeeCode && e.EmployeeId != id))
            {
                return BadRequest($"Employee with code '{employee.EmployeeCode}' already exists.");
            }

            if (await _context.Employees.AnyAsync(e => e.Email == employee.Email && e.EmployeeId != id))
            {
                return BadRequest($"Employee with email '{employee.Email}' already exists.");
            }

            existingEmployee.EmployeeCode = employee.EmployeeCode;
            existingEmployee.FullName = employee.FullName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Designation = employee.Designation;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.ProjectId = employee.ProjectId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
