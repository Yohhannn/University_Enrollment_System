using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Configuration;
using Npgsql;
using University_Enrollment_System.Models;

namespace University_Enrollment_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly string _connectionString;

        public AdminController()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;
        }

        public ActionResult MainAdmin()
        {
            List<int> statList = getDashboardStat();
            ViewBag.statList = statList;
            return View("~/Views/Admin/Dashboard.cshtml");
        }

        public ActionResult Admin_Curriculum()
        {
            try
            {
                var programs = GetProgramsFromDatabase();
                var academicYears = GetAcademicYearsFromDatabase(); 
                
                ViewBag.Programs = programs;
                ViewBag.AcademicYears = academicYears;
                
                return View("~/Views/Admin/Curriculum.cshtml");
            }
            catch (Exception ex)
            {
                // Log error (consider using ILogger)
                System.Diagnostics.Debug.WriteLine($"Error loading curriculum: {ex.Message}");
        
                // Pass an empty list to the view to prevent null reference exceptions
                ViewBag.AcademicYears = new List<AcademicYear>();
                return View("~/Views/Admin/Curriculum.cshtml", new List<Program>());
            }
        }


        public ActionResult Admin_Course()
        {
            // Redirect to CourseController.Index instead of duplicating logic
            return RedirectToAction("Course", "Course");
        }

        public ActionResult Admin_AddCourse()
        {
            return RedirectToAction("Index", "AddProgram"); // Use AddProgramController
        }

        public ActionResult Admin_EditCourse()
        {
            return View("~/Views/Admin/EditProgram.cshtml");
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

        private List<AcademicYear> GetAcademicYearsFromDatabase()
        {
            var academicYears = new List<AcademicYear>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"ay_code\", \"ay_start_year\", \"ay_end_year\" FROM \"academic_year\"", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            academicYears.Add(new AcademicYear
                            {
                                AyCode = reader.GetString(0),
                                AyStartYear = reader.GetInt16(1),
                                AyEndYear = reader.GetInt16(2),
                            });
                        }
                    }
                }
            }

            return academicYears;
        }

        private List<Semester> GetSemesterFromDatabase()
        {
            var semesters = new List<Semester>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"sem_id\", \"sem_name\" FROM \"semester\"", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            semesters.Add(new Semester
                            {
                                SemId = reader.GetInt16(0),
                                SemName = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return semesters;
        }
        
        private List<string> GetYearSemesterOptions()
        {
            var academicYears = GetAcademicYearsFromDatabase();
            var semesters = GetSemesterFromDatabase();

            var result = new List<string>();
                
            foreach (var ay in academicYears)
            {
                foreach (var sem in semesters)
                {
                    result.Add($"{ay.AyCode} - {sem.SemName}");
                }
            }

            return result;
        }

        public List<int> getDashboardStat()
        {
            var statList = new List<int>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                statList.Add(getStat(conn, "SELECT COUNT(*) FROM student"));
                statList.Add(getStat(conn, "SELECT COUNT(*) FROM Faculty WHERE isprofessor  IS TRUE"));
                statList.Add(getStat(conn, "SELECT COUNT(*) FROM Faculty WHERE isadmin IS TRUE"));
                statList.Add(getStat(conn, "SELECT COUNT(*) FROM Faculty WHERE isprogramhead IS TRUE"));
                statList.Add(getStat(conn, "SELECT COUNT(*) FROM Course"));
            }
            return statList;
        }

        public int getStat(NpgsqlConnection conn, string query)
        {
            int stat = 0;
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stat = reader.GetInt32(0);
                    }
                }   
            }
            return stat;
        }
        
        private List<Course> GetCoursesFromDatabase()
        {
            var courses = new List<Course>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            SELECT 
                c.""crs_code"", 
                c.""crs_title"", 
                c.""crs_units"", 
                c.""crs_lec"", 
                c.""crs_lab"", 
                c.""ctg_code"",
                COALESCE(cat.""ctg_name"", 'Uncategorized'),
                c.""preq_crs_code""
            FROM ""course"" c
            LEFT JOIN ""course_category"" cat ON c.""ctg_code"" = cat.""ctg_code""", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new Course
                            {
                                Crs_Code = !reader.IsDBNull(0) ? reader.GetString(0) : null,
                                Crs_Title = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                                Crs_Units = reader.GetDecimal(2),
                                Crs_Lec = reader.GetInt32(3),
                                Crs_Lab = reader.GetInt32(4),
                                Ctg_Code = !reader.IsDBNull(5) ? reader.GetString(5) : null,
                                Ctg_Name = !reader.IsDBNull(6) ? reader.GetString(6) : "Uncategorized",
                                Preq_Crs_Code = !reader.IsDBNull(7) ? reader.GetString(7) : null
                            });
                        }
                    }
                }
            }

            return courses;
        }
    }
}