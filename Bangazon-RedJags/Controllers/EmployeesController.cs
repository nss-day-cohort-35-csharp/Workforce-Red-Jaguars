using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon_RedJags.Models;
using Bangazon_RedJags.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Bangazon_RedJags.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employees
        public async Task<ActionResult> Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, d.Name DepartmentName
                                        FROM Employee e
                                        LEFT JOIN Department d
                                            ON e.DepartmentId = d.Id";

                    var reader = await cmd.ExecuteReaderAsync();

                    var employees = new List<EmployeeViewModel>();

                    int EmployeeIdOrdinal = reader.GetOrdinal("Id");
                    int FirstNameOrdinal = reader.GetOrdinal("FirstName");
                    int LastNameOrdinal = reader.GetOrdinal("LastName");
                    int DepartmentNameOrdinal = reader.GetOrdinal("DepartmentName");


                    while (reader.Read())
                    {
                        employees.Add(new EmployeeViewModel
                        {
                            Id = reader.GetInt32(EmployeeIdOrdinal),
                            FirstName = reader.GetString(FirstNameOrdinal),
                            LastName = reader.GetString(LastNameOrdinal),
                            DepartmentName = reader.GetString(DepartmentNameOrdinal)
                        });
                    }
                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            var trainings = GetAllTrainingPrograms().Select(p => new TrainingSelect
            {
                Name = p.Name,
                Id = p.Id,
                isSelected = false
            }).ToList();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT e.Id, e.FirstName, e.LastName, d.Name AS DepartmentName,
                                c.Make, c.Model, tp.Name AS TrainingName
                        FROM Employee e
                        LEFT JOIN Computer c ON ComputerId = c.Id
                        LEFT JOIN Department d ON e.DepartmentId = d.Id
                        LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.id
                        LEFT JOIN TrainingProgram tp ON et.TrainingProgramId = tp.Id
                        
                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    BasicEmployee employee = null;

                    while (reader.Read())
                    {
                        if (employee == null)
                        {


                            employee = new BasicEmployee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Computer = reader.GetString(reader.GetOrdinal("Make")) + " " + reader.GetString(reader.GetOrdinal("Model")),
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("TrainingName")))
                            {
                                employee.EmployeeTrainings.Add(reader.GetString(reader.GetOrdinal("TrainingName")));
                            }                 
                         } 
                        else if (!reader.IsDBNull(reader.GetOrdinal("TrainingName")))
                        {
                            employee.EmployeeTrainings.Add(reader.GetString(reader.GetOrdinal("TrainingName")));
                        }
                    }
                    reader.Close();

                    if (employee == null)
                    {
                        return NotFound($"No Employee found with the ID of {id}");
                    }
                    foreach (TrainingSelect training in trainings)
                    {
                        if (employee.EmployeeTrainings.Any(e => e == training.Name))
                        {
                            training.isSelected = true;
                        }
                    }

                    employee.TrainingList = trainings;


                    return View(employee);
                }
            }
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var departments = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();

            var isSupervisor = GetIsSupervisor().Select(d => new SelectListItem
            {
                Text = d.ToString(),
                Value = d.ToString()
            }).ToList();

            var computers = GetComputers(-1).Select(d => new SelectListItem
            {
                Text = d.Model,
                Value = d.Id.ToString()
            }).ToList();

            var viewModel = new EmployeeCreateModel
            {
                Employee = new Employee(),
                Departments = departments,
                IsSupervisor = isSupervisor,
                Computers = computers
            };

            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, DepartmentId, Email, IsSupervisor, ComputerId)
                                            VALUES (@firstName, @lastName, @departmentId, @email, @isSupervisor, @computerId)";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@email", employee.Email));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));

                        cmd.ExecuteNonQuery();
                    }
                }

                // RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            var department = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();

            var isSupervisor = GetIsSupervisor().Select(d => new SelectListItem
            {
                Text = d.ToString(),
                Value = d.ToString()
            }).ToList();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, DepartmentId, Email, IsSupervisor, ComputerId
                                        FROM Employee
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId"))
                        };
                        reader.Close();

                        var computers = GetComputers(employee.ComputerId).Select(d => new SelectListItem
                        {
                            Text = d.Model,
                            Value = d.Id.ToString()
                        }).ToList();

                        var viewModel = new EmployeeCreateModel
                        {
                            Employee = employee,
                            Departments = department,
                            IsSupervisor = isSupervisor,
                            Computers = computers
                        };
                        reader.Close();
                        return View(viewModel);
                    }
                    reader.Close();
                    return NotFound();
                }
            }
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Employee employee )
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            SET FirstName = @firstName, 
                                                LastName = @lastName, 
                                                DepartmentId = @departmentId,
                                                Email = @email,
                                                IsSupervisor = @isSupervisor,
                                                ComputerId = @computerId
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@email", employee.Email));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        /*if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        return BadRequest($"No Employee with Id of {id}");*/
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //return View();
                return RedirectToAction(nameof(Edit), new { id = id });
            }
        }

        // POST: Employees/EditTraining/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTraining(int id, BasicEmployee employee)
        {
            try
            {
                DeleteAllUpcomingTrianing(id);
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        foreach (TrainingSelect training in employee.TrainingList)
                            {
                               if (training.isSelected)
                            {
                                cmd.CommandText = @"INSERT INTO EmployeeTraining (TrainingProgramId, EmployeeId)
                                            OUTPUT INSERTED.id
                                            VALUES (@trainigProgramId, @employeeId)";

                                cmd.Parameters.Add(new SqlParameter("@trainigProgramId", training.Id));
                                cmd.Parameters.Add(new SqlParameter("@employeeId", employee.Id));


                                
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                            }
                            
                        }
                    }
                }

                // RedirectToAction("Index");
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        //
        //
        //
        private List<Department> GetDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Budget FROM Department";

                    var reader = cmd.ExecuteReader();

                    var departments = new List<Department>();

                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        });
                    }

                    reader.Close();

                    return departments;
                }
            }
        }


        private List<bool> GetIsSupervisor()
        {
            var list = new List<bool> { true, false };

            return list;
        }

        private List<Computer> GetComputers(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model)
                    cmd.CommandText = @"SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Model
                                            FROM Computer c
                                            LEFT JOIN Employee e
                                                ON c.Id = e.ComputerId
                                            WHERE e.ComputerId is Null";

                    var reader = cmd.ExecuteReader();

                    var computers = new List<Computer>();

                    if( id > 0 )
                    {
                        computers.Add( GetComputer( id ) );
                    }

                    while (reader.Read())
                    {
                        var computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model"))
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }

                        if (computer.DecomissionDate == null)
                        {
                            computers.Add(computer);
                        }
                    }
                    reader.Close();

                    return computers;
                }
            }
        }


            //helper function to grab all training programs
        private List<TrainingProgram> GetAllTrainingPrograms()
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, [Name], StartDate, EndDate, MaxAttendees 
                                        FROM TrainingProgram
                                        WHERE StartDate >= @today";
                        //
                        cmd.Parameters.Add(new SqlParameter("@today", DateTime.Now));
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                        int IdOrdinal = reader.GetOrdinal("Id");
                        int NameOrdinal = reader.GetOrdinal("Name");
                        int StartDateOrdinal = reader.GetOrdinal("StartDate");
                        int EndDateOrdinal = reader.GetOrdinal("EndDate");
                        int MaxAttendeesOrdinal = reader.GetOrdinal("MaxAttendees");

                        while (reader.Read())
                        {
                            TrainingProgram trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(IdOrdinal),
                                Name = reader.GetString(NameOrdinal),
                                StartDate = reader.GetDateTime(StartDateOrdinal),
                                EndDate = reader.GetDateTime(EndDateOrdinal),
                                MaxAttendees = reader.GetInt32(MaxAttendeesOrdinal)
                            };


                            trainingPrograms.Add(trainingProgram);
                        }
                        reader.Close();

                        return trainingPrograms;
                    }
                }
            }

        private Computer GetComputer(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Model
                                            FROM Computer
                                            WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    //Computer computer = new Computer();

                    //while (reader.Read())
                    //{
                    reader.Read();
                    Computer computer = new Computer
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                        Make = reader.GetString(reader.GetOrdinal("Make")),
                        Model = reader.GetString(reader.GetOrdinal("Model"))
                    };
                    if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                    {
                        computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                    }
                    //}
                    reader.Close();
                    return computer;
                }
            }
        }

        //helper function to delete all upcoming trainings for employee
        private void DeleteAllUpcomingTrianing(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE et FROM EmployeeTraining et
                                            LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                            WHERE EmployeeId = @eId AND StartDate >= @today";
                        cmd.Parameters.Add(new SqlParameter("@eId", id));
                        cmd.Parameters.Add(new SqlParameter("@today", DateTime.Now));

                        cmd.ExecuteNonQuery();
                        
                    }
                }
            }
            catch (Exception)
            {

                throw;
                
            }
        }

    }
}