using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Npgsql;
using University_Enrollment_System.Models;

namespace University_Enrollment_System.Controllers
{


    public class StudentEnrollmentController : Controller
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;


        [HttpGet]
        public JsonResult GetAvailableSchedules(string cur_year_level, string cur_semester, string prog_code)
        {
            try
            {
                var schedules = new List<ScheduleViewModel>();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    s.schd_id,
                    s.crs_code,
                    c.crs_title,
                    s.room,
                    s.prof -- assuming prof is stored directly in schedule table
                FROM schedule s
                JOIN course c ON s.crs_code = c.crs_code
                JOIN curriculum_course cc ON cc.crs_code = s.crs_code
                WHERE cc.cur_year_level = @cur_year_level
                  AND cc.cur_semester = @cur_semester
                  AND cc.prog_code = @prog_code";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cur_year_level", cur_year_level);
                        cmd.Parameters.AddWithValue("@cur_semester", cur_semester);
                        cmd.Parameters.AddWithValue("@prog_code", prog_code);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                schedules.Add(new ScheduleViewModel
                                {
                                    SchdId = Convert.ToInt32(reader["schd_id"]),
                                    CrsCode = reader["crs_code"]?.ToString(),
                                    CourseTitle = reader["crs_title"]?.ToString(),
                                    Room = reader["room"]?.ToString() ?? "N/A",
                                    Prof = reader["prof"]?.ToString() ?? "TBA" // fallback if null
                                });
                            }
                        }
                    }
                }

                return Json(schedules, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAvailableSubjects(string cur_year_level, string cur_semester, string prog_code)
        {
            try
            {
                var subjects = new List<SubjectViewModel>();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
            SELECT 
                s.crs_code,
                c.crs_title,
                se.tsl_start_time,
                se.tsl_end_time,
                se.tsl_day,
                s.room,
                c.crs_units,
                c.crs_lec,
                c.crs_lab
            FROM schedule s
            JOIN course c ON s.crs_code = c.crs_code
            JOIN session se ON s.schd_id = se.schd_id
            JOIN curriculum_course cc ON cc.crs_code = s.crs_code
            WHERE cc.cur_year_level = @cur_year_level
              AND cc.cur_semester = @cur_semester
              AND cc.prog_code = @prog_code";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cur_year_level", cur_year_level);
                        cmd.Parameters.AddWithValue("@cur_semester", cur_semester);
                        cmd.Parameters.AddWithValue("@prog_code", prog_code);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dayNum = Convert.ToInt32(reader["tsl_day"]);
                                var dayStr = dayNum switch
                                {
                                    1 => "M",
                                    2 => "T",
                                    3 => "W",
                                    4 => "Th",
                                    5 => "F",
                                    _ => "N/A"
                                };

                                var startTime = reader["tsl_start_time"] == DBNull.Value
                                    ? ""
                                    : reader["tsl_start_time"].ToString();
                                var endTime = reader["tsl_end_time"] == DBNull.Value
                                    ? ""
                                    : reader["tsl_end_time"].ToString();
                                var room = reader["room"]?.ToString() ?? "N/A";

                                subjects.Add(new SubjectViewModel
                                {
                                    CourseCode = reader["crs_code"]?.ToString(),
                                    Title = reader["crs_title"]?.ToString(),
                                    Time = $"{startTime} - {endTime}",
                                    Days = dayStr,
                                    Room = room,
                                    Units = Convert.ToInt32(reader["crs_units"])
                                });
                            }
                        }
                    }
                }

                return Json(subjects, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetSubjectSessions(string courseCode)
        {
            try
            {
                // Replace this with real DB query based on your schema
                var sessions = new List<SubjectSession>();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    tsl_day AS Day,
                    tsl_start_time AS StartTime,
                    tsl_end_time AS EndTime,
                    room AS Room
                FROM schedule s
                JOIN session se ON s.schd_id = se.schd_id
                WHERE s.crs_code = @courseCode";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@courseCode", courseCode);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Convert day number to name if needed
                                string day = reader["Day"].ToString() switch
                                {
                                    "0" => "Sunday",
                                    "1" => "Monday",
                                    "2" => "Tuesday",
                                    "3" => "Wednesday",
                                    "4" => "Thursday",
                                    "5" => "Friday",
                                    "6" => "Saturday",
                                    _ => "Unknown"
                                };

                                string startTime = TimeSpan.Parse(reader["StartTime"].ToString()).ToString(@"hh\:mm");
                                string endTime = TimeSpan.Parse(reader["EndTime"].ToString()).ToString(@"hh\:mm");

                                sessions.Add(new SubjectSession
                                {
                                    CourseCode = courseCode,
                                    Day = day,
                                    Time = $"{startTime} - {endTime}",
                                    Room = reader["Room"]?.ToString()
                                });
                            }
                        }
                    }
                }

                return Json(sessions, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult Student_Enrollment()
        {
            var sessionStudCode = Session["Stud_Code"];

            if (sessionStudCode == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int studCode;
            if (!int.TryParse(sessionStudCode.ToString(), out studCode))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Load student data
                var student = GetStudentById(studCode);
                if (student == null)
                {
                    ViewBag.ErrorMessage = "Student not found.";
                    return View("~/Views/Shared/Error.cshtml");
                }

                // Load programs for dropdown
                var programs = GetProgramsFromDatabase();
                ViewBag.Programs = programs;
                var academicYears = GetAcademicYears();
                ViewBag.AcademicYears = academicYears;

                // Return the view with student model and programs in ViewBag
                return View("~/Views/Main/StudentEnroll.cshtml", student);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading student data: {ex.Message}";
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult EnrollmentSuccess()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
        // var sessionStudCode = Session["Stud_Code"];
            //
            // if (sessionStudCode == null)
            // {
            //     return RedirectToAction("Login", "Account");
            // }
            //
            // int studCode;
            // if (!int.TryParse(sessionStudCode.ToString(), out studCode))
            // {
            //     return RedirectToAction("Login", "Account");
            // }
            //
            // try
            // {
            //     // Load student data
            //     var student = GetStudentById(studCode);
            //     if (student == null)
            //     {
            //         ViewBag.ErrorMessage = "Student not found.";
            //         return View("~/Views/Shared/Error.cshtml");
            //     }
            //
            //     // Load programs for dropdown
            //     var programs = GetProgramsFromDatabase();
            //     ViewBag.Programs = programs;
            //     var academicYears = GetAcademicYears();
            //     ViewBag.AcademicYears = academicYears;
            //
            //     // Return the view with student model and programs in ViewBag
            //     return View("~/Views/Main/StudentEnroll.cshtml", student);
            // }
            // catch (Exception ex)
            // {
            //     ViewBag.ErrorMessage = $"Error loading student data: {ex.Message}";
            //     return View("~/Views/Shared/Error.cshtml");
            // }

    private Student GetStudentById(int studCode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM STUDENT WHERE STUD_CODE = @studCode";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@studCode", studCode);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Student
                            {
                                Stud_Id = reader.GetInt32(reader.GetOrdinal("stud_id")),
                                Stud_Lname = reader["stud_lname"]?.ToString(),
                                Stud_Fname = reader["stud_fname"]?.ToString(),
                                Stud_Mname = reader["stud_mname"]?.ToString(),
                                Stud_Dob = Convert.ToDateTime(reader["stud_dob"]),
                                Stud_Contact = reader["stud_contact"]?.ToString(),
                                Stud_Email = reader["stud_email"]?.ToString(),
                                Stud_Address = reader["stud_address"]?.ToString(),
                                Stud_Code = Convert.ToInt32(reader["stud_code"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        private List<Program> GetProgramsFromDatabase()
        {
            var programs = new List<Program>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"prog_code\", \"prog_title\" FROM \"program\"", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            programs.Add(new Program
                            {
                                ProgCode = reader.GetString(0),
                                ProgTitle = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return programs;
        }
        private List<AcademicYear> GetAcademicYears()
        {
            var academicYears = new List<AcademicYear>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT ay_code, ay_start_year, ay_end_year FROM academic_year", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            academicYears.Add(new AcademicYear
                            {
                                AyCode = reader.GetString(0),
                                AyStartYear = reader.GetInt32(1),
                                AyEndYear = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }

            return academicYears;
        }
        
        
        
        [HttpPost]
        public JsonResult SubmitEnrollment(EnrollmentView model)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                INSERT INTO enrollment (enrol_status, enrol_date, enrol_yr_level, enrol_sem, stud_id, ay_code)
                VALUES (@status, @date, @yrlevel, @sem, @studId, @ayCode)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", model.EnrolStatus);
                        cmd.Parameters.AddWithValue("@date", model.EnrolDate);
                        cmd.Parameters.AddWithValue("@yrlevel", model.EnrolYrLevel);
                        cmd.Parameters.AddWithValue("@sem", model.EnrolSem);
                        cmd.Parameters.AddWithValue("@studId", model.StudId);
                        // cmd.Parameters.AddWithValue("@crsCode", model.CrsCode);
                        cmd.Parameters.AddWithValue("@ayCode", model.AyCode);

                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Enrollment submitted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


    }
}