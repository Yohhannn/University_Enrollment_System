using System.Web.Mvc;
using System.Web.Routing;


namespace University_Enrollment_System
{
    public class RouteConfig
    {
        
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Add this specific route before the default route
            routes.MapRoute(
                name: "TeacherClassesRoute",
                url: "Teacher/Classes",
                defaults: new { controller = "Teacher", action = "Classes" }
            );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Add this specific route before the default route
            routes.MapRoute(
                name: "TeacherDashboardRoute",
                url: "Teacher/Dashboard",
                defaults: new { controller = "Teacher", action = "Dashboard" }
            );
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "ProgramHeadClassManagementRoute",
                url: "Head/ManageClass",
                defaults: new { controller = "ProgramHead", action = "ClassManagement" }
            );
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "ProgramHeadStudentManagementRoute",
                url: "Head/ManageStudent",
                defaults: new { controller = "ProgramHead", action = "StudentManagement" }
            );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "ProgramHeadSetScheduleListRoute",
                url: "Head/Schedules",
                defaults: new { controller = "ProgramHead", action = "Schedule" }
            );
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "ProgramHeadEnrollmentApprovalListRoute",
                url: "Head/EnrollmentApproval",
                defaults: new { controller = "ProgramHead", action = "Approval" }
            );

            
                        
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "ProgramHeadViewStudentListRoute",
                url: "Head/Students",
                defaults: new { controller = "ProgramHead", action = "Students" }
            );
            
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "ProgramHeadDashboardRoute",
                url: "Head/Dashboard",
                defaults: new { controller = "ProgramHead", action = "Dashboard" }
            );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            
            routes.MapRoute(
                name: "LoginFacultyRoute",
                url: "Auth/LoginFaculty",
                defaults: new { controller = "Auth", action = "LoginFaculty" }
            );
            
            routes.MapRoute(
                name: "AdminEditCourseRoute",
                url: "Admin/Course/EditCourse",
                defaults: new { controller = "Admin", action = "Admin_EditCourse" }
            );
            
            routes.MapRoute(
                name: "AdminAddCourseRoute",
                url: "Admin/Course/AddCourse",
                defaults: new { controller = "AddProgram", action = "Index" }
            );
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "AdminCourseRoute",
                url: "Admin/Course",
                defaults: new { controller = "Course", action = "Index" }
            );
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "AdminCurriculumRoute",
                url: "Admin/Curriculum",
                defaults: new { controller = "Admin", action = "Admin_Curriculum" }
            );
            
            routes.MapRoute(
                name: "SignUpEntry",
                url: "Auth/Entry",
                defaults: new { controller = "Auth", action = "Entry" }
            );
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainAdminRoute",
                url: "Admin/Dashboard",
                defaults: new { controller = "Admin", action = "MainAdmin" }
            );
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainSchedule;eRoute",
                url: "Home/Schedule",
                defaults: new { controller = "Main", action = "Student_Schedule" }
            );
            
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainViewGradeRoute",
                url: "Home/Grades",
                defaults: new { controller = "Main", action = "Student_Grade" }
            );
            
            routes.MapRoute(
                name: "LoginStudentRoute",
                url: "Auth/LoginStudent",
                defaults: new { controller = "Auth", action = "LoginStudent" }
            );
            
          
            // Add this specific route before the default route

            routes.MapRoute(
                name: "StudentEnrollment",
                url: "Main/Student_Enrollment",
                defaults: new { controller = "StudentEnrollment", action = "Enrollment" }
            );
            
            
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainProfileRoute",
                url: "Home/Profile",
                defaults: new { controller = "Main", action = "Student_Profile" }
            );
            
           
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainHomeRoute",
                url: "Main/Home",
                defaults: new { controller = "Main", action = "MainHome" }
            );
                        
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "SignUpRoute",
                url: "SignUp",
                defaults: new { controller = "Login", action = "SignUp" }
            );
            
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginAdminRoute",
                url: "Login/Admin",
                defaults: new { controller = "Login", action = "LoginAdmin" }
            );
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginHeadRoute",
                url: "Login/Head",
                defaults: new { controller = "Login", action = "LoginHead" }
            );

            
           
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginTeacherRoute",
                url: "Login/Teacher",
                defaults: new { controller = "Login", action = "LoginTeacher" }
            );
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginRoute",
                url: "Login/Student",
                defaults: new { controller = "Login", action = "Login" }
            );
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "GetAllCourses",
                url: "Course/GetAllCourses",
                defaults: new { controller = "Course", action = "GetAllCourses" }
            );

            routes.MapRoute(
                name: "GetAssignedCourses",
                url: "CurriculumCourse/GetAssignedCourses",
                defaults: new { controller = "CurriculumCourse", action = "GetAssignedCourses" }
            );

            routes.MapRoute(
                name: "AssignCourses",
                url: "CurriculumCourse/AssignCourses",
                defaults: new { controller = "CurriculumCourse", action = "AssignCourses" }
            );

        }
    }
}