using System.Collections.Generic;

namespace University_Enrollment_System.Models;

public class Program
{
    public string ProgCode { get; set; }
    public string ProgTitle { get; set; }

    // Navigation Properties
    public ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
}