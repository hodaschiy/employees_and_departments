using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.Arm;
using WebApp.Data;

namespace WebApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        [StringLength(150, MinimumLength = 3)]
        [RegularExpression(@".*[^.].*")]
        public string Name { get; set; }
        public int? ChiefId { get; set; }
        [ForeignKey ("Department")]
        public int? ParentId { get; set; }
        public string? Tree { get; set; }
        [NotMapped]
        public int? Level { get => Tree!.Where(x => x == '.').Count(); }
        public virtual Employee? Chief { get; set; }
        public virtual ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
        public virtual Department? Parent { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public Department() { }

        public Department(Department dep) //
        { 
            Id = dep.Id;
            Name = dep.Name;
            ChiefId = dep.ChiefId;
            ParentId = dep.ParentId;
            Tree = dep.Tree;

            Chief = dep.Chief;
            ChildDepartments = dep.ChildDepartments;
            Parent = dep.Parent;
            Employees = dep.Employees;
        }
    }

    public class DepartmentView : Department
    {
        public ICollection<Department> HChildDepartments { get; set; }

        public DepartmentView(Department dep, WebAppContext context) : base (dep)
        {
            HChildDepartments = context.Department.Where(dep => dep.Tree.StartsWith(Tree+'.')).OrderBy(dep => dep.Tree).Include(d => d.Chief).ToList();
            //Level = Tree.Where(x => x == '.').Count();
        }
    }
}
