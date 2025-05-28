using System.Web.Mvc;

namespace University_Enrollment_System.Controllers.Auth
{
    public class LoginController : Controller
    {
        // GET: /Login
        public ActionResult Login()
        {
            // View is located at Views/Auth/Login.cshtml
            return View("~/Views/Auth/Login.cshtml");
        }

        public ActionResult LoginTeacher()
        {
            return View("~/Views/Auth/LoginTeacher.cshtml");
        }

        public ActionResult LoginHead()
        {
            return View("~/Views/Auth/LoginProgramHead.cshtml");
        }

        public ActionResult LoginAdmin()
        {
            return View("~/Views/Auth/LoginAdmin.cshtml");
        }

        public ActionResult SignUp()
        {
            return View("~/Views/Auth/SignUp.cshtml");
        }
    }
}