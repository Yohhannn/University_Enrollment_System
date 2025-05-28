using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Npgsql;
using University_Enrollment_System.Models;

namespace University_Enrollment_System.Controllers
{
    public class CurriculumCourseController : Controller
    {
        [HttpPost]
public JsonResult AssignCourses(List<CurriculumCourse> courses)
{
    if (courses == null || courses.Count == 0)
        return Json(new { success = false, message = "No course data provided." });

    var first = courses[0];
    var success = false;
    var message = "";

    try
    {
        using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString))
        {
            conn.Open();

            // Get existing course codes for this curriculum
            var existingCourses = new HashSet<string>();
            using (var cmd = new NpgsqlCommand(@"
                SELECT crs_code FROM curriculum_course
                WHERE prog_code = @prog_code AND cur_year_level = @year AND cur_semester = @semester AND ay_code = @ay", conn))
            {
                cmd.Parameters.AddWithValue("@prog_code", first.ProgCode);
                cmd.Parameters.AddWithValue("@year", first.CurYearLevel);
                cmd.Parameters.AddWithValue("@semester", first.CurSemester);
                cmd.Parameters.AddWithValue("@ay", first.AyCode);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingCourses.Add(reader.GetString(0));
                    }
                }
            }

            var incomingCodes = new HashSet<string>(courses.ConvertAll(c => c.CrsCode));

            // Courses to delete
            foreach (var oldCourse in existingCourses)
            {
                if (!incomingCodes.Contains(oldCourse))
                {
                    using (var cmd = new NpgsqlCommand(@"
                        DELETE FROM curriculum_course
                        WHERE crs_code = @code AND prog_code = @prog_code AND cur_year_level = @year AND cur_semester = @semester AND ay_code = @ay", conn))
                    {
                        cmd.Parameters.AddWithValue("@code", oldCourse);
                        cmd.Parameters.AddWithValue("@prog_code", first.ProgCode);
                        cmd.Parameters.AddWithValue("@year", first.CurYearLevel);
                        cmd.Parameters.AddWithValue("@semester", first.CurSemester);
                        cmd.Parameters.AddWithValue("@ay", first.AyCode);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // Courses to insert
            foreach (var course in courses)
            {
                if (!existingCourses.Contains(course.CrsCode))
                {
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO curriculum_course (cur_code, crs_code, cur_year_level, cur_semester, ay_code, prog_code)
                        VALUES (@cur_code, @crs_code, @year, @semester, @ay, @prog_code)", conn))
                    {
                        cmd.Parameters.AddWithValue("@cur_code", course.CurCode);
                        cmd.Parameters.AddWithValue("@crs_code", course.CrsCode);
                        cmd.Parameters.AddWithValue("@year", course.CurYearLevel);
                        cmd.Parameters.AddWithValue("@semester", course.CurSemester);
                        cmd.Parameters.AddWithValue("@ay", course.AyCode);
                        cmd.Parameters.AddWithValue("@prog_code", course.ProgCode);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            success = true;
            message = "Courses updated successfully.";
        }
    }
    catch (Exception ex)
    {
        message = "An error occurred: " + ex.Message;
    }

    return Json(new { success, message });
}

        
        [HttpGet]
        public JsonResult GetAssignedCourses(string progCode, string yearLevel, string semester, string ayCode)
        {
            var courses = new List<object>();
            using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString))
            {
                conn.Open();
                string sql = @"
            SELECT c.crs_code, cr.crs_title
            FROM curriculum_course c
            INNER JOIN course cr ON c.crs_code = cr.crs_code
            WHERE c.prog_code = @progCode AND c.cur_year_level = @yearLevel AND c.cur_semester = @semester AND c.ay_code = @ayCode";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@progCode", progCode);
                    cmd.Parameters.AddWithValue("@yearLevel", yearLevel);
                    cmd.Parameters.AddWithValue("@semester", semester);
                    cmd.Parameters.AddWithValue("@ayCode", ayCode);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new
                            {
                                code = reader.GetString(0),
                                title = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return Json(courses, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetAcademicYears(string progCode)
        {
            if (string.IsNullOrEmpty(progCode))
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            try
            {
                var academicYears = new List<object>();

                using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT DISTINCT ay.ay_code, ay.ay_start_year, ay.ay_end_year
                        FROM curriculum c
                        INNER JOIN academic_year ay ON c.ay_code = ay.ay_code
                        WHERE c.prog_code = @prog_code
                        ORDER BY ay.ay_start_year";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@prog_code", progCode);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ayCode = reader.GetString(0);
                                var startYear = reader.GetInt32(1);
                                var endYear = reader.GetInt32(2);
                                academicYears.Add(new
                                {
                                    AyCode = ayCode,
                                    DisplayText = $"{startYear}-{endYear}"
                                });
                            }
                        }
                    }
                }

                return Json(academicYears, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
