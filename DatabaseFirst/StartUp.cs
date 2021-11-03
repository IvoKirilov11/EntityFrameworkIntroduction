using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var softUNiContext = new SoftUniContext();
            var result = DeleteProjectById(softUNiContext);
            Console.WriteLine(result);
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(x => new 
                { 
                  x.EmployeeId,
                  x.FirstName,
                  x.LastName,
                  x.MiddleName,
                  x.JobTitle,
                  x.Salary
                
                })
                .OrderBy(x => x.EmployeeId)
                .ToList();


                var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }
            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(s => s.Salary > 50000)
                .Select(x => new
                {
                    x.FirstName,
                    x.Salary
                })
                .OrderBy(n => n.FirstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} - {item.Salary:F2}");
            }
            var result = sb.ToString().TrimEnd();
            return result;

        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
               .Where(s => s.Department.Name == "Research and Development")
               .Select(x => new
               {
                   x.FirstName,
                   x.LastName,
                   x.Department.Name,
                   x.Salary
               })
               .OrderBy(x => x.Salary)
               .ThenByDescending(x => x.FirstName)
               .ToList();

            var sb = new StringBuilder();

            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} from {item.Name} - ${item.Salary:F2}");
            }
            var result = sb.ToString().TrimEnd();
            return result;


        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(address);
            context.SaveChanges();

            var nakov = context.Employees
                .FirstOrDefault(x => x.LastName == "Nakov");
            nakov.AddressId = address.AddressId;
            context.SaveChanges();

            var addresses = context.Employees
                .Select(x => new
                {
                    x.Address.AddressText,
                    x.Address.AddressId
                })
                .OrderByDescending(a => a.AddressId)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in addresses)
            {
                sb.AppendLine(item.AddressText);
            }
            var result = sb.ToString().TrimEnd();
            return result;


        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
               .Include(x => x.EmployeesProjects)
               .ThenInclude(x => x.Project)
               .Where(x => x.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003))
               .Select(x => new
               {

                  EmployeeFirstname = x.FirstName,
                   EmployeeLastname = x.LastName,
                   ManagerFirstName = x.Manager.FirstName,
                   ManagerLastName = x.Manager.LastName,
                   Projects = x.EmployeesProjects.Select(p => new 
                   { 
                       ProjectName = p.Project.Name,
                       StartData = p.Project.StartDate,
                       EndDate = p.Project.EndDate
                   })
               })
               .Take(10)
               .ToList();


               var sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.EmployeeFirstname} {emp.EmployeeLastname} - Manager: {emp.ManagerFirstName} {emp.ManagerLastName}");

                foreach (var project in emp.Projects)
                {
                    if(project.EndDate.HasValue)
                    {
                        sb.AppendLine($"--{project.ProjectName} - {project.StartData} - {project.EndDate}");
                    }
                    else
                    {
                        sb.AppendLine($"--{project.ProjectName} - {project.StartData} - not finished");
                    }
                    
                }
            }
            var result = sb.ToString().TrimEnd();
            return result;

        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var emploeey147 = context.Employees
                .Select(x => new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects.OrderBy(x=>x.Project.Name).Select(p => new 
                    { 
                        p.Project.Name
                    })
                    
                })
                .FirstOrDefault(x => x.EmployeeId == 147);

            var sb = new StringBuilder();

            sb.AppendLine($"{emploeey147.FirstName} {emploeey147.LastName} - {emploeey147.JobTitle}");
            
            foreach (var proects in emploeey147.Projects)
            {
                sb.AppendLine($"{proects.Name}");
            }
            var result = sb.ToString().TrimEnd();
            return result;



        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var adresses = context.Addresses
                .Select(x => new
                {
                    x.AddressText,
                    x.Town.Name,
                    x.Employees.Count
                })

                .OrderByDescending(x => x.Count)
                .ThenBy(t => t.Name)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var adress in adresses)
            {
                sb.AppendLine($"{adress.AddressText}, {adress.Name} - {adress.Count} employees");
            }
            var result = sb.ToString().TrimEnd();
            return result;

        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    x.Name,
                    x.Manager.FirstName,
                    x.Manager.LastName,
                    Employees = x.Employees.Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList()
                })
                .ToList();
               

            var sb = new StringBuilder();

            foreach (var departmant in departments)
            {
                sb.AppendLine($"{departmant.Name} - {departmant.FirstName} {departmant.LastName}");

                foreach (var emploeey in departmant.Employees)
                {
                    sb.AppendLine($"{emploeey.FirstName} {emploeey.LastName} - {emploeey.JobTitle}");
                }
            }
            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var departmants = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services"};

            var emploeeys = context.Employees
                .Where(x => departmants.Contains(x.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e=>e.LastName)
                .ToList();

            foreach (var emploeey in emploeeys)
            {
                emploeey.Salary *= 1.12m;
            }

            var sb = new StringBuilder();

            foreach (var emploeey in emploeeys)
            {
                sb.AppendLine($"{emploeey.FirstName} {emploeey.LastName} (${emploeey.Salary:F2})");
            }
            var result = sb.ToString().TrimEnd();
            return result;


        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var emploeeys = context.Employees
                .Where(x => x.FirstName.StartsWith("Sa"))
                .Select(x=> new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    x.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var emploeey in emploeeys)
            {
                sb.AppendLine($"{emploeey.FirstName} {emploeey.LastName} - {emploeey.JobTitle} - (${emploeey.Salary:F2})");
            }
            var result = sb.ToString().TrimEnd();
            return result;

        }
        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns
                .Include(x=>x.Addresses)
                .FirstOrDefault(x => x.Name == "Seattle");
            var allAdressID = town.Addresses.Select(x => x.AddressId).ToList();
            var emploees = context.Employees
                .Where(x => x.AddressId.HasValue && allAdressID.Contains(x.AddressId.Value))
                .ToList();

            foreach (var emploeey in emploees)
            {
                emploeey.AddressId = null;
            }


            foreach (var adressId in allAdressID)
            {
                var adress = context.Addresses
                    .FirstOrDefault(x => x.AddressId == adressId);
                
                context.Addresses.Remove(adress);
            }
            context.Towns.Remove(town);

            context.SaveChanges();

            return $"{allAdressID.Count} addresses in Seattle were deleted";
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var project = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                   StartDate= x.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                })
                .OrderBy(x=>x.Name)
                .ToList();
            var sb = new StringBuilder();

            foreach (var pro in project)
            {
                sb.AppendLine($"{pro.Name}");
                sb.AppendLine($"{pro.Description}");
                sb.AppendLine($"{pro.StartDate}");
            }
            var result = sb.ToString().TrimEnd();
            return result;




        }



        public static string DeleteProjectById(SoftUniContext context)
        {



            var epCol = context.EmployeesProjects.Where(x => x.ProjectId == 2).ToList();
            foreach (var item in epCol)
            {
                context.Remove(item);
                
            }


            var projectToDelete = context.Projects.FirstOrDefault(a => a.ProjectId == 2);
            
            context.Projects.Remove(projectToDelete);


            var projects = context.Projects
                .Take(10)
                .ToList();


            

            var sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
            }
            var result = sb.ToString().TrimEnd();
            return result;

        }
    }
}
