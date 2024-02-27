using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEmployeeApp.Core
{
    public class Employee
    {

        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(128)]
        public string? EmployeeName { get; set; }

        [Required]
        public int EmployeeSalary { get; set; }
    }
}