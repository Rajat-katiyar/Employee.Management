using Employee.Domain.Entities;

namespace Employee.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Domain.Entities.Employee>> GetAllEmployeesAsync();
        Task<Domain.Entities.Employee?> GetEmployeeByIdAsync(int id);
        Task CreateEmployeeAsync(Domain.Entities.Employee employee);
        Task UpdateEmployeeAsync(Domain.Entities.Employee employee);
        Task DeleteEmployeeAsync(int id);
    }
}
