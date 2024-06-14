using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using WebApp.Data;

namespace WebApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChiefId { get; set; }
        public int DepartmentId { get; set; }
        public string? Tree { get; set; }
        public virtual Employee? Chief {get; set;}
        public virtual Department Department { get; set; }
        public virtual Employee? Subordinate { get; set; }
    }
    public class EmpoloyeeView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChiefId { get; set; }
        public string? ChiefName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        EmpoloyeeView(Employee emp, WebAppContext context)
        {
            Id = emp.Id;
            Name = emp.Name;
            ChiefId = emp.ChiefId;
            ChiefName = context.Employee.Where(em => em.Id == emp.Id).Select(em => em.Name).FirstOrDefault();
            DepartmentId = emp.DepartmentId;
            DepartmentName = context.Department.Where(dep => dep.Id == emp.DepartmentId).Select(dep => dep.Name).FirstOrDefault()!;
        }
        public static EmpoloyeeView? CreateEmpoloyeeView(int empId, WebAppContext context)
        {
            Employee? employee = context.Employee.Where(em => em.Id == empId).FirstOrDefault();
            return employee == null ? null : new EmpoloyeeView(employee!, context);
        }
    }
}
