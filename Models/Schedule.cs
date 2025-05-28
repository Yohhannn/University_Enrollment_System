using System.ComponentModel.DataAnnotations;

namespace University_Enrollment_System.Models;

public class Schedule
{
    public int SchdId { get; set; }
    
    [Required]
    public string CrsCode { get; set; }

    public string Room { get; set; }
    public string Description { get; set; }

    // New property:
    public string CrsTitle { get; set; }  // This holds crs_title from Course table
}