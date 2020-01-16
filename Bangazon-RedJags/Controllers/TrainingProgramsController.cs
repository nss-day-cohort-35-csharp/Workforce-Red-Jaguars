using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon_RedJags.Models;
using Bangazon_RedJags.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Bangazon_RedJags.Controllers
{
    public class TrainingProgramsController : Controller
    {

        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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

        // GET: TrainingPrograms
        public ActionResult Index()
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

                    return View(trainingPrograms);
                }
            }
        }

        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT tp.Id AS TrainingId, tp.[Name] AS TrainingName, tp.StartDate, tp.EndDate, tp.MaxAttendees, e.FirstName, e.LastName, e.Id AS EmployeeId
                        FROM TrainingProgram tp
                        LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = tp.Id
                        LEFT JOIN Employee e ON et.EmployeeId = e.Id
                        WHERE tp.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingWithEmployees trainingProgram = null;

                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                            trainingProgram = new TrainingWithEmployees
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TrainingId")),
                                Name = reader.GetString(reader.GetOrdinal("TrainingName")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                            {
                                var employee = new BasicEmployee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                };

                                trainingProgram.EmployeesAttending.Add(employee);

                            }
                        } else if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                        {
                            var employee = new BasicEmployee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            };

                            trainingProgram.EmployeesAttending.Add(employee);
                        }
                    }
                    reader.Close();
                    

                    if (trainingProgram == null)
                    {
                        return NotFound($"No Training Program found with the ID of {id}");
                    }

                    return View(trainingProgram);
                }
            }
            
        }

        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram trainingProgram
            )
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (trainingProgram.StartDate < DateTime.Now)
                        {
                            ViewBag.ErrorMessage1 = "STOP.  We can't go back to the future.  Please adjust start date accordingly";
                            return View();
                        }
                        else if (trainingProgram.StartDate > trainingProgram.EndDate)
                        {
                            ViewBag.ErrorMessage2 = "STOP.  Collaborate & listen.  The ending date must be after the session.";
                            return View();
                        }
                        else
                        {
                            cmd.CommandText = @"INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees)
                                            OUTPUT INSERTED.id
                                            VALUES (@name, @startDate, @endDate, @maxAttendees)";

                            cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                            cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                            cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                            cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));

                            int newId = (int)cmd.ExecuteScalar();
                            trainingProgram.Id = newId;

                            
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], StartDate, EndDate, MaxAttendees 
                        FROM TrainingProgram
                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }
                    reader.Close();

                    if (trainingProgram == null)
                    {
                        return NotFound($"No Training Program found with the ID of {id}");
                    }
                    if (trainingProgram.EndDate < DateTime.Now)
                    {
                        TempData["ErrorMessage"] = "Sorry.  You cant edit the past."; 
                        return RedirectToAction(nameof(Index));
                    }

                    return View(trainingProgram);
                }
            }
        }

        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram trainingProgram)
        {
            if (trainingProgram.StartDate < DateTime.Now)
            {
                ViewBag.ErrorMessage1 = "STOP.  We can't go back to the future.  Please adjust start date accordingly";
                return View();
            }
            else if (trainingProgram.StartDate > trainingProgram.EndDate)
            {
                ViewBag.ErrorMessage2 = "STOP.  Collaborate & listen.  The ending date must be after the session.";
                return View();
            }
            else
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE TrainingProgram
                                                SET [Name] = @name,
                                                    StartDate = @startDate,
                                                    EndDate = @endDate,
                                                    MaxAttendees = @maxAttendees
                                                WHERE Id = @id";

                            cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                            cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                            cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                            cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                return RedirectToAction(nameof(Index));
                            }
                            ViewBag.ErrorMessage3 = "Sorry, that Program could not be edited";
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    
                }
                catch
                {
                    return View();
                }
            }
        }

        // GET: TrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingPrograms/Delete/5
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
    }
}