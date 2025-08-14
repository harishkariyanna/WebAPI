using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeProjectTrackerAPI.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Project Code is required")]
        [StringLength(10, ErrorMessage = "Project Code cannot exceed 10 characters")]
        public string ProjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project Name is required")]
        [StringLength(100, ErrorMessage = "Project Name cannot exceed 100 characters")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Budget is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive value")]
        public decimal Budget { get; set; }

        // JsonIgnore prevents circular references during JSON serialization
        [JsonIgnore]
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
