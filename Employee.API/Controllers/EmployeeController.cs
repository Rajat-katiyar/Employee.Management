using Employee.Application.Interfaces;
using Employee.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Employee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(new { message = "Employees retrieved successfully.", data = employees });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) 
            {
                return NotFound(new { message = "Employee not found." });
            }
            return Ok(new { message = "Employee retrieved successfully.", data = employee });
        }

        [HttpPost]
        public async Task<ActionResult> Create(Domain.Entities.Employee employee)
        {
            await _employeeService.CreateEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, new { message = "Employee created successfully.", data = employee });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Domain.Entities.Employee employee)
        {
            if (id != employee.Id) return BadRequest(new { message = "ID mismatch." });

            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound(new { message = "Employee not found." });
            }

            await _employeeService.UpdateEmployeeAsync(employee);
            return Ok(new { message = "Employee updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound(new { message = "Employee not found." });
            }

            await _employeeService.DeleteEmployeeAsync(id);
            return Ok(new { message = "Employee deleted successfully." });
        }
    }
}
