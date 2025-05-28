using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Npgsql;
using University_Enrollment_System.Models;

namespace University_Enrollment_System.Controllers
{
    public class CourseController : Controller
    {
        private readonly string _connectionString;

        public CourseController()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;
        }

        public ActionResult Course()
        {
            var courses = GetCoursesFromDatabase(); 
            return View("~/Views/Admin/Courses.cshtml", courses);
        }

        // GET: /Course/Create
        public ActionResult Create()
        {
            return View("~/Views/Admin/AddProgram.cshtml");
        }

        // GET: /Course/Edit/{id}
        public ActionResult Edit(string id)
        {
            var course = GetCourseById(id);
            if (course == null) return HttpNotFound();

            return View("~/Views/Admin/EditProgram.cshtml", course);
        }

        // GET: /Course/GetAllCourses
        [HttpGet]
        public JsonResult GetAllCourses(string progCode = null, string ayCode = null)
        {
            var courses = new List<dynamic>();
            try
            {
                using (var db = new NpgsqlConnection(_connectionString))
                {
                    db.Open();
                    using (var cmd = new NpgsqlCommand(@"
                        SELECT 
                            c.crs_code, 
                            c.crs_title, 
                            COALESCE(cat.ctg_name, 'General') AS category,
                            COALESCE(p.preq_crs_code, 'None') AS prerequisite,
                            c.crs_units, 
                            c.crs_lec, 
                            c.crs_lab
                        FROM course c
                        LEFT JOIN course_category cat ON c.ctg_code = cat.ctg_code
                        LEFT JOIN prerequisite p ON p.crs_code = c.crs_code
                    ", db))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                courses.Add(new
                                {
                                    code = reader["crs_code"].ToString(),
                                    title = reader["crs_title"].ToString(),
                                    category = reader["category"].ToString(),
                                    prerequisite = reader["prerequisite"].ToString(),
                                    units = reader["crs_units"] != DBNull.Value ? Convert.ToDecimal(reader["crs_units"]) : 0,
                                    lec = reader["crs_lec"] != DBNull.Value ? Convert.ToInt32(reader["crs_lec"]) : 0,
                                    lab = reader["crs_lab"] != DBNull.Value ? Convert.ToInt32(reader["crs_lab"]) : 0
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                Console.WriteLine($"Error fetching courses: {ex.Message}");
            }

            return Json(courses, JsonRequestBehavior.AllowGet);
        }

        private List<Course> GetCoursesFromDatabase()
        {
            var courses = new List<Course>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    SELECT 
                        c.crs_code, 
                        c.crs_title, 
                        COALESCE(cat.ctg_name, 'General') AS category,
                        p.preq_crs_code,
                        c.crs_units, 
                        c.crs_lec, 
                        c.crs_lab
                    FROM course c
                    LEFT JOIN course_category cat ON c.ctg_code = cat.ctg_code
                    LEFT JOIN prerequisite p ON p.crs_code = c.crs_code", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new Course
                            {
                                Crs_Code = reader["crs_code"]?.ToString(),
                                Crs_Title = reader["crs_title"]?.ToString(),
                                Ctg_Name = reader["category"]?.ToString(),
                                Preq_Crs_Code = reader["preq_crs_code"]?.ToString(),
                                Crs_Units = reader["crs_units"] != DBNull.Value ? Convert.ToDecimal(reader["crs_units"]) : 0,
                                Crs_Lec = reader["crs_lec"] != DBNull.Value ? Convert.ToInt32(reader["crs_lec"]) : 0,
                                Crs_Lab = reader["crs_lab"] != DBNull.Value ? Convert.ToInt32(reader["crs_lab"]) : 0
                            });
                        }
                    }
                }
            }

            return courses;
        }

        private object GetCourseById(string id)
        {
            // Implement logic to fetch single course by ID
            // For now, just returning null
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCourse(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Invalid course ID.";
                return RedirectToAction("Course");
            }

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmdCurriculum = new NpgsqlCommand(
                                "DELETE FROM \"curriculum_course\" WHERE \"crs_code\" = @id", conn, transaction))
                            {
                                cmdCurriculum.Parameters.AddWithValue("id", id);
                                cmdCurriculum.ExecuteNonQuery();
                            }
                            
                            using (var cmdCourse = new NpgsqlCommand(
                                "DELETE FROM \"course\" WHERE \"crs_code\" = @id", conn, transaction))
                            {
                                cmdCourse.Parameters.AddWithValue("id", id);
                                int rowsAffected = cmdCourse.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    TempData["SuccessMessage"] = $"Course '{id}' deleted successfully.";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    TempData["ErrorMessage"] = $"Course '{id}' not found.";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw; // Re-throw so outer block can log it
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting course: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the course.";
            }

            return RedirectToAction("Course");
        }
    }
}