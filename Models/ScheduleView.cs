namespace University_Enrollment_System.Models;

public class ScheduleViewModel
{
    public int SchdId { get; set; }
    public string CrsCode { get; set; }
    public string CourseTitle { get; set; } // From course table
    public string Room { get; set; }
    public string Prof { get; set; } // Assuming prof is from another table or stored directly
}