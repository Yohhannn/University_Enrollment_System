using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Policy;
using System.Web.Mvc;
using Npgsql;
using University_Enrollment_System.Models;

namespace University_Enrollment_System.Controllers
{
    public class AddProgramController : Controller
    {
        private readonly ICourseRepository _courseRepository;

        public AddProgramController()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;
            _courseRepository = new CourseRepository(connectionString);
        }

        // GET: /AddProgram/
        public ActionResult Index()
        {
            try
            {
                ViewBag.CourseCategories = _courseRepository.GetCourseCategories();
                ViewBag.CoursesForPrereq = _courseRepository.GetAllCourses();
                return View("~/Views/Admin/AddProgram.cshtml");
            }
            catch (Exception ex)
            {
                // Log error here
                return View("Error");
            }
        }

        // POST: /AddProgram/AddCourseAjax
        [HttpPost]
        public JsonResult AddCourseAjax(Course course)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input data." });
            }

            try
            {
                var result = _courseRepository.AddCourse(course);
                return result;
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "An error occurred while saving the course.",
                    error = ex.Message 
                });
            }
        }
    }

    public interface ICourseRepository
    {
        JsonResult AddCourse(Course course);
        List<CourseCategory> GetCourseCategories();
        List<Course> GetAllCourses();
    }

    public class CourseRepository : ICourseRepository
    {
        private readonly string _connectionString;

        public CourseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResult AddCourse(Course course)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Check for duplicate course code
                        if (CourseCodeExists(conn, course.Crs_Code))
                        {
                            return new JsonResult
                            {
                                Data = new { 
                                    success = false, 
                                    message = "A course with this code already exists." 
                                }
                            };
                        }

                        // Check for duplicate course title
                        if (CourseTitleExists(conn, course.Crs_Title))
                        {
                            return new JsonResult
                            {
                                Data = new { 
                                    success = false, 
                                    message =  "A course with this title already exists." 
                                }
                            };
                        }
                        InsertMainCourse(conn, course);

                        transaction.Commit();
                        return new JsonResult
                        {
                            Data = new { success = true, message = "Course added successfully!", redirectUrl = "/Admin/Admin_Course"
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new JsonResult
                        {
                            Data = new { 
                                success = false, 
                                message = "An error occurred while saving the course.",
                                error = ex.Message 
                            }
                        };
                    }
                }
            }
        }

        private bool CourseCodeExists(NpgsqlConnection conn, string code)
        {
            const string sql = "SELECT 1 FROM COURSE WHERE CRS_CODE = @code";
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("code", code);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool CourseTitleExists(NpgsqlConnection conn, string title)
        {
            const string sql = "SELECT 1 FROM COURSE WHERE CRS_TITLE = @title";
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("title", title);
                return cmd.ExecuteScalar() != null;
            }
        }

        private void InsertMainCourse(NpgsqlConnection conn, Course course)
        {
            const string insertCourseSql = @"
        INSERT INTO COURSE (
            CRS_CODE, 
            CRS_TITLE, 
            CTG_CODE, 
            PREQ_ID, 
            CRS_UNITS, 
            CRS_LEC, 
            CRS_LAB
        ) VALUES (
            @code, 
            @title, 
            @ctgCode, 
            @prereq, 
            @units, 
            @lec, 
            @lab)";

            using (var cmd = new NpgsqlCommand(insertCourseSql, conn))
            {
                cmd.Parameters.AddWithValue("code", course.Crs_Code);
                cmd.Parameters.AddWithValue("title", course.Crs_Title);
                cmd.Parameters.AddWithValue("ctgCode", course.Ctg_Code);

                // Check if prerequisite is empty or null
                if (string.IsNullOrEmpty(course.Preq_Crs_Code))
                    cmd.Parameters.AddWithValue("prereq", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("prereq", course.Preq_Crs_Code);

                cmd.Parameters.AddWithValue("units", course.Crs_Units);
                cmd.Parameters.AddWithValue("lec", course.Crs_Lec);
                cmd.Parameters.AddWithValue("lab", course.Crs_Lab);

                cmd.ExecuteNonQuery();
            }
        }

        

        public List<CourseCategory> GetCourseCategories()
        {
            const string sql = "SELECT CTG_CODE, CTG_NAME FROM COURSE_CATEGORY";
            return ExecuteQuery<CourseCategory>(sql, reader => new CourseCategory
            {
                Ctg_Code = reader["CTG_CODE"]?.ToString(),
                Ctg_Name = reader["CTG_NAME"]?.ToString()
            });
        }

        public List<Course> GetAllCourses()
        {
            const string sql = "SELECT CRS_CODE, CRS_TITLE FROM COURSE ORDER BY CRS_CODE";
            return ExecuteQuery<Course>(sql, reader => new Course
            {
                Crs_Code = reader["CRS_CODE"]?.ToString(),
                Crs_Title = reader["CRS_TITLE"]?.ToString()
            });
        }

        private List<T> ExecuteQuery<T>(string sql, Func<NpgsqlDataReader, T> mapper)
        {
            var results = new List<T>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(mapper(reader));
                        }
                    }
                }
            }

            return results;
        }
    }
}