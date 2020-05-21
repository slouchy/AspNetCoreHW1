using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHW1.Models;
using Microsoft.Data.SqlClient;

namespace AspNetCoreHW1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<ActionResult<Department>> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@DepartmentID", id),
                new SqlParameter("@Name", department.Name),
                new SqlParameter("@Budget", department.Budget),
                new SqlParameter("@StartDate", department.StartDate),
                new SqlParameter("@InstructorID", department.InstructorId),
                new SqlParameter("@RowVersion_Original", department.RowVersion)
            };

            var result = _context.Database.ExecuteSqlRaw("EXECUTE Department_Update @DepartmentID, @Name, @Budget, @StartDate, @InstructorID, @RowVersion_Original", sqlParameters.ToArray());
            if (result != 1)
            {
                return Conflict();
            }

            department = await _context.Department.FirstOrDefaultAsync(x => x.DepartmentId == id);
            return department;
        }

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Name", department.Name),
                new SqlParameter("@Budget", department.Budget),
                new SqlParameter("@StartDate", DateTime.Now),
                new SqlParameter("@InstructorID", department.InstructorId),
            };

            var result = await _context.Database.ExecuteSqlRawAsync("EXECUTE [Department_Insert] @Name, @Budget, @StartDate, @InstructorID", sqlParameters.ToArray());
            if (result != 1)
            {
                return Conflict();
            }

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@DepartmentID", department.DepartmentId),
                new SqlParameter("@RowVersion_Original", department.RowVersion)
            };

            var result = await _context.Database.ExecuteSqlRawAsync("EXECUTE [Department_Delete] @DepartmentID, @RowVersion_Original", sqlParameters.ToArray());
            if (result != 1)
            {
                return Conflict();
            }

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
