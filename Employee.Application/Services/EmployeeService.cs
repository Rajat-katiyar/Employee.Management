using Employee.Application.Interfaces;
using Employee.Domain.Entities;

namespace Employee.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Domain.Entities.Employee>> GetAllEmployeesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Domain.Entities.Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateEmployeeAsync(Domain.Entities.Employee employee)
        {
            await _repository.AddAsync(employee);
        }

        public async Task UpdateEmployeeAsync(Domain.Entities.Employee employee)
        {
            await _repository.UpdateAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
