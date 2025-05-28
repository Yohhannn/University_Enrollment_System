namespace University_Enrollment_System.Models
{
    public class CurriculumCourse
    {
        public string CurCode { get; set; }
        public string CrsCode { get; set; }
        public string CurYearLevel { get; set; }
        public string CurSemester { get; set; }
        public string AyCode { get; set; }
        public string ProgCode { get; set; }

        // Navigation Properties (optional for EF navigation)
        public Course Course { get; set; }
        public Program Program { get; set; }
    }
}