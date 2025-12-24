using Employee.Application.Interfaces;
using Employee.Domain.Entities;

namespace Employee.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IKafkaProducer _kafkaProducer;

        public EmployeeService(IEmployeeRepository repository, IKafkaProducer kafkaProducer)
        {
            _repository = repository;
            _kafkaProducer = kafkaProducer;
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
            await _kafkaProducer.ProduceAsync("employee-events", $"New employee created: {employee.Name} ({employee.Email})");
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
