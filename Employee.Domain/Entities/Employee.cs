namespace Employee.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; } 
        public string? City { get; set; }
        public DateTime Creation { get; set; } = DateTime.UtcNow;

    }
}
