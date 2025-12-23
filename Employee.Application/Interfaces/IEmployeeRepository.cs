using Employee.Domain.Entities;

namespace Employee.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Domain.Entities.Employee>> GetAllAsync();
        Task<Domain.Entities.Employee?> GetByIdAsync(int id);
        Task AddAsync(Domain.Entities.Employee employee);
        Task UpdateAsync(Domain.Entities.Employee employee);
        Task DeleteAsync(int id);
    }
}
