namespace Employee.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; } 
        public string? City { get; set; }
        public DateTime Creation { get; set; } = DateTime.UtcNow;
    }
}
