using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Data;
using System;
using System.Linq;

namespace WebApp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WebAppContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<WebAppContext>>()))
            {
                if (context.Department.Any() || context.Employee.Any())
                {
                    return;   // DB has been seeded
                }
                context.Department.AddRange(
                    new Department
                    {
                        Id = 1, 
                        Name = "Дирекция", 
                    }
                );
                context.SaveChanges();

                Department depr = new Department
                {
                    Id = 2,
                    Name = "Производство",
                    ParentId = 1
                };

                context.Department.AddRange(
                    new Department
                    {
                        Id = 2 ,
                        Name = "Производство" ,
                        ParentId = 1
                    },
                    new Department
                    {
                        Id = 3,
                        Name = "Снабжение",
                        ParentId = 1
                    }
                );

                context.SaveChanges();

                context.Department.AddRange(
                    new Department
                    {
                        Id = 4,
                        Name = "Цех1",
                        ParentId = 2
                    },
                    new Department
                    {
                        Id = 5,
                        Name = "Цех2",
                        ParentId = 2
                    }
                );
                context.Department.AddRange(
                    new Department
                    {
                        Id = 6,
                        Name = "Склад",
                        ParentId = 3
                    },
                    new Department
                    {
                        Id = 7,
                        Name = "Кадры",
                        ParentId = 3
                    },
                    new Department
                    {
                        Id = 8,
                        Name = "Бухгалтерия",
                        ParentId = 3
                    }
                );
                context.SaveChanges();

                context.Employee.AddRange(
                    new Employee { Name = "Климов Иван Иванович", DepartmentId = 1},
                    new Employee { Name = "Сергеева Мелисса Дамировна", DepartmentId = 1 },
                    new Employee { Name = "Моисеева Полина Дмитриевна", DepartmentId = 2 },
                    new Employee { Name = "Жуков Никита Александрович", DepartmentId = 2 },
                    new Employee { Name = "Смирнов Николай Артёмович", DepartmentId = 3 },
                    new Employee { Name = "Фомичев Даниил Сергеевич", DepartmentId = 3 },
                    new Employee { Name = "Кудрявцев Александр Романович", DepartmentId = 4 },
                    new Employee { Name = "Петров Егор Владимирович", DepartmentId = 4 },
                    new Employee { Name = "Фролова Евгения Михайловна", DepartmentId = 5 },
                    new Employee { Name = "Князев Михаил Даниилович", DepartmentId = 5 },
                    new Employee { Name = "Кондрашов Василий Игнатиевич", DepartmentId = 6 },
                    new Employee { Name = "Петров Сергей Гордеевич", DepartmentId = 6 },
                    new Employee { Name = "Корнилова Дарья Ивановна", DepartmentId = 7 },
                    new Employee { Name = "Николаева Николь Ивановна", DepartmentId = 8 },
                    new Employee { Name = "Пономарев Никита Егорович", DepartmentId = 8 }
                );
                context.SaveChanges();

                var employees = context.Employee.ToArray();
                var departments = context.Department.ToArray();
                for ( int i = 0; i < departments.Length; i++) {
                    departments[i].ChiefId = employees.Where(empl => empl.Department == departments[i]).FirstOrDefault().Id;
                }
                context.UpdateRange(departments);
                for (int i = 0; i < employees.Length; i++)
                {
                    employees[i].Tree = departments.Where(dep => dep == employees[i].Department).FirstOrDefault()!.Tree;
                }
                employees[3].Chief = employees[1];
                employees[5].Chief = employees[2];
                employees[7].Chief = employees[0];
                employees[10].Chief = employees[1];
                employees[11].Chief = employees[5];
                employees[13].Chief = employees[7];
                employees[14].Chief = employees[9];

                context.UpdateRange(employees);
                context.SaveChanges();
            }
        }
    }
}