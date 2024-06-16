using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
}
