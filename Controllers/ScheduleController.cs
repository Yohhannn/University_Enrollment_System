using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Npgsql;
using University_Enrollment_System.Models;

namespace University_Enrollment_System.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;

        public ActionResult Index()
        {
            // ✅ Fetch course data from database
            var courseList = GetCourseFromDatabase();
            var scheduleList = GetScheduleFromDatabase();
            var sessionList = GetSessionFromDatabase();
            
            // ✅ Pass course data to ViewBag
            ViewBag.Course = courseList;
            ViewBag.Schedules = scheduleList;
            ViewBag.Sessions = sessionList;

            // ✅ Return view after setting ViewBag
            return View("~/Views/Program-Head/SetSchedules.cshtml");
        }

        private List<Course> GetCourseFromDatabase()
        {
            var courseList = new List<Course>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"crs_code\", \"crs_title\" FROM \"course\"", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courseList.Add(new Course
                            {
                                Crs_Code = reader.GetString(0),
                                Crs_Title = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return courseList;
        }
        
        private List<Schedule> GetScheduleFromDatabase()
        {
            var schedList = new List<Schedule>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            SELECT s.schd_id, s.crs_code, s.room, c.crs_title 
            FROM schedule s
            JOIN course c ON s.crs_code = c.crs_code", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            schedList.Add(new Schedule
                            {
                                SchdId = reader.GetInt32(0),
                                CrsCode = reader.GetString(1),
                                Room = reader.GetString(2),
                                CrsTitle = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return schedList;
        }
        
        private List<Session> GetSessionFromDatabase()
        {
            var sessionList = new List<Session>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM session", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sessionList.Add(new Session
                            {
                                SsnId = reader.GetInt32(0),
                                TslStartTime = reader.GetTimeSpan(1),
                                TslEndTime = reader.GetTimeSpan(2),
                                TslDay = reader.GetInt32(3),
                                SchdId = reader.GetInt32(4),
                            });
                        }
                    }
                }
            }

            return sessionList;;
        }

        
        [HttpPost]
        public ActionResult Add(Schedule sched)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Invalid input data." });
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            
            using var insertCmd = conn.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO schedule (
                    crs_code, room
                ) VALUES (
                    @crsCode, @room
                ) RETURNING schd_id";

            insertCmd.Parameters.AddWithValue("crsCode", sched.CrsCode.Trim());
            insertCmd.Parameters.AddWithValue("room", sched.Room.Trim());

            var newId = (int)(insertCmd.ExecuteScalar())!;

            return Json(new
            {
                success = true,
                data = new { Id = newId, CrsCode = sched.CrsCode.Trim(), Room = sched.Room.Trim() }
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Unexpected error", error = ex.Message });
        }
    }
        
        [HttpPost]
        public ActionResult AddSession(Session session)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid input data." });
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
            
                using var insertCmd = conn.CreateCommand();
                insertCmd.CommandText = @"
                INSERT INTO session (
                    tsl_start_time, tsl_end_time, tsl_day, schd_id
                ) VALUES (
                    @tslStartTime, @tslEndTime, @tslDay, @schdId
                ) RETURNING session_id";

                insertCmd.Parameters.AddWithValue("tslStartTime", session.TslStartTime);
                insertCmd.Parameters.AddWithValue("tslEndTime", session.TslEndTime);
                insertCmd.Parameters.AddWithValue("tslDay", session.TslDay);
                insertCmd.Parameters.AddWithValue("schdId", session.SchdId);

                var newId = (int)(insertCmd.ExecuteScalar())!;

                return Json(new
                {
                    success = true,
                    data = new {  Id = newId,
                                  TslStartTime = session.TslStartTime,
                                  TslEndTime = session.TslEndTime, 
                                  TslDay = session.TslDay, 
                                  SchdId = session.SchdId }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Unexpected error", error = ex.Message });
            }
        }
    }
}