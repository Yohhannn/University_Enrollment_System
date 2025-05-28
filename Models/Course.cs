namespace University_Enrollment_System.Models
{
    public class Course
    {
        public string Crs_Code { get; set; }
        public string Crs_Title { get; set; }
        public decimal Crs_Units { get; set; }
        public int Crs_Lec { get; set; }       
        public int Crs_Lab { get; set; }        
        public string Ctg_Code { get; set; }    
        public string Ctg_Name { get; set; } 
        public string Preq_Crs_Code { get; set; }
        
    }
}