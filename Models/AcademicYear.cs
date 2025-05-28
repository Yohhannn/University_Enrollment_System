using System.Collections.Generic;

namespace University_Enrollment_System.Models;

public class AcademicYear
{
    public string AyCode { get; set; }
    public int AyStartYear { get; set; }
    public int AyEndYear { get; set; }

    // Navigation Properties
    public ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
}