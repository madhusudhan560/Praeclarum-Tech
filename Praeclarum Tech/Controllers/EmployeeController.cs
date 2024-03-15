using Praeclarum_Tech.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Praeclarum_Tech.Controllers
{
    //[Authorize]
    public class EmployeeController : Controller
    {
        private readonly TaskEntities _dbContext = new TaskEntities();

       
        [HttpPost]
        public async Task<JsonResult> SaveEmployee( string Name, string Gender, int Age, string Position, string Office, DateTime HireDate, int Salary)
        {
            try
            {
             
                    Employee employee = new Employee();
                    employee.Name = Name;
                    employee.Gender = Gender;
                    employee.Age = Age;
                    employee.Position = Position;
                    employee.Office = Office;
                    employee.HireDate = HireDate;
                    employee.Salary = Salary;
                    _dbContext.Employees.Add(employee);
                

                await _dbContext.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Save exception details to a text file
                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string logFolderPath = Path.Combine(desktopFolder, "Praeclarum_Tech_Error_Logs");
                string logFilePath = Path.Combine(logFolderPath, "ErrorLog.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // Create directory if not exists
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("Date: " + DateTime.Now);
                    writer.WriteLine("Error Message: " + ex.Message);
                    writer.WriteLine("Stack Trace: " + ex.StackTrace);
                    writer.WriteLine();
                }
                // Return error message as JSON
                return Json(new { success = false, errorMessage = "An error occurred while saving employee details." });
            }
        }
        public ActionResult ViewDetail()
        {
            try
            {

                var list = _dbContext.Employees.ToList();
                return View(list);
            }
            catch (Exception ex)
            {
                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string logFolderPath = Path.Combine(desktopFolder, "Praeclarum_Tech_Error_Logs");
                string logFilePath = Path.Combine(logFolderPath, "ErrorLog.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // Create directory if not exists
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("Date: " + DateTime.Now);
                    writer.WriteLine("Error Message: " + ex.Message);
                    writer.WriteLine("Stack Trace: " + ex.StackTrace);
                    writer.WriteLine();
                }
                // Return error message as JSON
                return Json(new { success = false, errorMessage = "An error occurred while saving employee details." });
            }
        }



        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Delete(int id)
        {
            try
            {

                var employee = _dbContext.Employees.Find(id);


                if (employee == null)
                {

                    return Json(new { success = false, message = "Employee not found" });
                }


                _dbContext.Employees.Remove(employee);


                _dbContext.SaveChanges();


                return Json(new { success = true, message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {

                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string logFolderPath = Path.Combine(desktopFolder, "Praeclarum_Tech_Error_Logs");
                string logFilePath = Path.Combine(logFolderPath, "ErrorLog.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // Create directory if not exists
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("Date: " + DateTime.Now);
                    writer.WriteLine("Error Message: " + ex.Message);
                    writer.WriteLine("Stack Trace: " + ex.StackTrace);
                    writer.WriteLine();
                }
                // Return error message as JSON
                return Json(new { success = false, errorMessage = "An error occurred while saving employee details." });
            }
        }


        [HttpGet]
        public ActionResult GetEmployeeDetails(int id)
        {
           
            try
            {
                using (var dbContext = new TaskEntities())
                {

                    var employee = dbContext.Employees.Find(id);
                    if (employee != null)
                    {


                        return Json(new 
                        {
                            id = employee.Id,
                            name = employee.Name,
                            position = employee.Position,
                            hiredate = employee.HireDate,
                            office = employee.Office,
                            salary = employee.Salary,
                            age = employee.Age,
                            success = true

                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                       

                        return Json(new { success = false, message = "Employee detail Not Get" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string logFolderPath = Path.Combine(desktopFolder, "Praeclarum_Tech_Error_Logs");
                string logFilePath = Path.Combine(logFolderPath, "ErrorLog.txt");
                // Create directory if it doesn't exist
                Directory.CreateDirectory(logFolderPath);

                // Write to log file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("Date: " + DateTime.Now);
                    writer.WriteLine("Error Message: " + ex.Message);
                    writer.WriteLine("Stack Trace: " + ex.StackTrace);
                    writer.WriteLine();
                }

                // Return error message as JSON
                return Json(new { success = false, errorMessage = "An error occurred while saving employee details." });

            }


        }

        [HttpPost]
        public JsonResult UpdateEmployeeDetails(int employeeId, string newName, int newage, string newposition, string newoffice, DateTime newhiredate, int newsalary)
        {
            try
            {

                var employee = _dbContext.Employees.Find(employeeId);

                if (employee == null)
                {

                    return Json(new { success = false, message = "Employee not found" });
                }


                employee.Name = newName;
                employee.Age = newage;
                employee.Position = newposition;
                employee.Office = newoffice;
                employee.HireDate = newhiredate;
                employee.Salary = newsalary;


                _dbContext.SaveChanges();


                return Json(new { success = true, message = "Employee details updated successfully" });
            }
            catch (Exception ex)
            {

                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string logFolderPath = Path.Combine(desktopFolder, "Praeclarum_Tech_Error_Logs");
                string logFilePath = Path.Combine(logFolderPath, "ErrorLog.txt");
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("Date: " + DateTime.Now);
                    writer.WriteLine("Error Message: " + ex.Message);
                    writer.WriteLine("Stack Trace: " + ex.StackTrace);
                    writer.WriteLine();
                }
                // Return error message as JSON
                return Json(new { success = false, errorMessage = "An error occurred while saving employee details." });
            }
        }

    }
}