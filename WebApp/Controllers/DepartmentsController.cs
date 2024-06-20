using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using WebApp.Data;
using WebApp.Models;
using static WebApp.Controllers.DepartmentsController;

namespace WebApp.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly WebAppContext _context;

        public DepartmentsController(WebAppContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var webAppContext = _context.Department.Include(d => d.Chief).Include(d => d.Parent);
            return View(await webAppContext.OrderBy(dep => dep.Tree).ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department
                .Include(d => d.Chief)
                .Include(d => d.Parent)
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(new DepartmentView(department, _context));
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["ChiefId"] = new SelectList(_context.Employee, "Id", "Id");
            ViewData["ParentId"] = new SelectList(_context.Department, "Id", "Id");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ChiefId,ParentId,Tree")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChiefId"] = new SelectList(_context.Employee, "Id", "Id", department.ChiefId);
            ViewData["ParentId"] = new SelectList(_context.Department, "Id", "Id", department.ParentId);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department.Where(dep => dep.Id == id).Include(dep => dep.Employees).FirstOrDefaultAsync();
            if (department == null)
            {
                return NotFound();
            }
            
            return View(new DepartmentEditView() { 
                                                    Department = new DepartmentView(department, _context), 
                                                    DepEmpls = new SelectList(department.Employees.Select(emp => new { emp.Id, emp.Name }), "Id", "Name"),
                                                    Departments = new SelectList(_context.Department.Where(dep => !(dep.Level == department.Level+1 && dep.Tree!.StartsWith(department.Tree!)) && !(department.Tree!.StartsWith(dep.Tree)) ).Select(dep => new { dep.Id, dep.Name }).ToList(), "Id", "Name"), 
                                                    Employees = new SelectList(_context.Employee.Where(emp => emp.Department != department).Select(emp => new { emp.Id, emp.Name }).ToList(), "Id", "Name"),
                                                    PotentialParentDeps = new SelectList(_context.Department.Where(dep => !(dep.Tree!.StartsWith(department.Tree!)) && dep != department).Select(dep => new { dep.Id, dep.Name }).ToList(), "Id", "Name")
            } 
            );
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ChiefId,ParentId,Tree")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChiefId"] = new SelectList(_context.Employee, "Id", "Id", department.ChiefId);
            ViewData["ParentId"] = new SelectList(_context.Department, "Id", "Id", department.ParentId);
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddChild(int id, int newChild, ViewType VT)
        {
            var childDep = await _context.Department.FindAsync(newChild);

            if (childDep == null)
            {
                return NotFound();
            }

            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            childDep.Parent = department;

            _context.Update(childDep);
            await _context.SaveChangesAsync();
            var routeValues = new RouteValueDictionary();
            routeValues.Add("id", id);

            switch (VT)
            {
                case ViewType.Self:
                    routeValues.Add("department", department);
                    return RedirectToAction(nameof(Edit), routeValues);
                    break;
                case ViewType.Child:
                    routeValues.Add("department", childDep);
                    return RedirectToAction(nameof(Edit), routeValues);
                    break;
                default:
                    return RedirectToAction(nameof(Index));
                    break;
            }


        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department
                .Include(d => d.Chief)
                .Include(d => d.Parent)
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(new DepartmentView(department, _context));
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department != null)
            {
                _context.Department.Remove(department);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.Id == id);
        }

        public enum ViewType
        {
            Self = 1,
            Child = 2,
            Parent = 3
        }
    }
}
