using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Enrollment_System.Models;

public class EnrollmentView
{
    public string EnrolStatus { get; set; }
    public DateTime EnrolDate { get; set; }
    public string EnrolYrLevel { get; set; }
    public string EnrolSem { get; set; }
    public int StudId { get; set; }
    public string AyCode { get; set; }
    public string BlockSection { get; set; }
    public string StudentStatus { get; set; }

    // List of selected schedules
    public List<ScheduleViewModel> Schedules { get; set; }
}
