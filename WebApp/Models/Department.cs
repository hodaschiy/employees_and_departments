using Microsoft.EntityFrameworkCore;

namespace WebApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChiefId { get; set; }
        public int? ParentId { get; set; }
        public string? Tree { get; set; }
        public virtual ICollection<Department>? ChildDepartments { get; set; }
        public virtual Department? Parent { get; set; }
        public virtual ICollection<Employee>? Employees { get; set; }
    }
}
