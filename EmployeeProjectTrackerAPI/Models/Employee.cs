using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EmployeeProjectTrackerAPI.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee Code is required")]
        [StringLength(8, ErrorMessage = "Employee Code cannot exceed 8 characters")]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(150, ErrorMessage = "Full Name cannot exceed 150 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters")]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Project assignment is required")]
        public int ProjectId { get; set; }

        // JsonIgnore prevents circular references
        [ForeignKey("ProjectId")]
        [JsonIgnore]
        public virtual Project? Project { get; set; } = null!;
    }
}
