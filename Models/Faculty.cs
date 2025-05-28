namespace University_Enrollment_System.Models
{
    public class Faculty
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsProfessor { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProgramHead { get; set; }
    }
}