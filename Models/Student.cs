namespace University_Enrollment_System.Models

{

    using System;

    using System.Collections.Generic;

    using System.Linq;

    using System.Web;

    public class Student

    {

        public int Stud_Id { get; set; }

        public string Stud_Lname { get; set; }

        public string Stud_Fname { get; set; }

        public string Stud_Mname { get; set; }

        public DateTime Stud_Dob { get; set; }

        public string Stud_Contact { get; set; }

        public string Stud_Email { get; set; }

        public string Stud_Address { get; set; }

        public string Stud_Password { get; set; }

        public DateTime Created_At { get; set; }

        public int Stud_Code { get; set; }

    }







}