using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApp.Data;

namespace WebApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        [RegularExpression(@".*[^.].*")]
        public string Name { get; set; }
        public int? ChiefId { get; set; }
        public int? ParentId { get; set; }
        public string? Tree { get; set; }
        public virtual ICollection<Department>? ChildDepartments { get; set; }
        public virtual Department? Parent { get; set; }
        public virtual ICollection<Employee>? Employees { get; set; }
    }
    public class DepartmentSimpleView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChiefId { get; set; }
        public string? ChiefName { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public int Level { get; set; }
        public string Tree { get; set; }

        public DepartmentSimpleView(Department dep, WebAppContext context)
        { 
            Id = dep.Id;
            Name = dep.Name;
            ChiefId = dep.ChiefId;
            ChiefName = context.Employee.Where(emp => emp.Id == dep.ChiefId).Select(emp => emp.Name).FirstOrDefault();
            ParentId = dep.ParentId;
            ParentName = context.Department.Where(dp => dp.Id == dep.ParentId).Select(dp => dp.Name).FirstOrDefault();
            Tree = dep.Tree!;
            Level = dep.Tree!.Where(x => x == '.').Count();
        }
        public static DepartmentSimpleView? CreateDepartmentSimpleView(int depId, WebAppContext context)
        {
            Department? department = context.Department.Where(dp => dp.Id == depId).FirstOrDefault();
            return department == null ? null : new DepartmentSimpleView(department!, context);
        }
    }
    public class DepartmentDetailedView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChiefId { get; set; }
        public string? ChiefName { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public string Tree { get; set; }
        public int Level { get; set; }
        public List<Tuple<int,string, string, int>> ChildDepartments { get; set; }
        public List<Tuple<int, string>> Employees { get; set; }

        public DepartmentDetailedView(Department dep, WebAppContext context)
        {
            Id = dep.Id;
            Name = dep.Name;
            ChiefId = dep.ChiefId;
            ChiefName = context.Employee.Where(emp => emp.Id == dep.ChiefId).Select(emp => emp.Name).FirstOrDefault();
            ParentId = dep.ParentId;
            ParentName = context.Department.Where(dp => dp.Id == dep.ParentId).Select(dp => dp.Name).FirstOrDefault();
            Tree = dep.Tree!;
            Level = dep.Tree!.Where(x => x == '.').Count();
            ChildDepartments = context.Department.Where(dp => dp.Tree!.StartsWith(dep.Tree!+'.')).ToList().Select(dp => new Tuple<int, string, string, int>(dp.Id, dp.Name, dp.Tree!, dp.Tree!.Where(x => x == '.').Count() )).OrderBy(dp => dp.Item3).ToList();
            Employees = context.Employee.Where(emp => emp.DepartmentId == dep.Id).Select(emp => new Tuple<int, string>(emp.Id, emp.Name)).ToList();
        }
        public static DepartmentDetailedView? CreateDepartmentDetailedView(int depId, WebAppContext context)
        {
            Department? department = context.Department.Where(dp => dp.Id == depId).FirstOrDefault();
            return department == null ? null : new DepartmentDetailedView(department!, context);
        }
    }
}
