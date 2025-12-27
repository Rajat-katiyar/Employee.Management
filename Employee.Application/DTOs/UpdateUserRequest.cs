using System.ComponentModel.DataAnnotations;

namespace Employee.Application.DTOs
{
    public class UpdateUserRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? FullName { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }
    }
}

