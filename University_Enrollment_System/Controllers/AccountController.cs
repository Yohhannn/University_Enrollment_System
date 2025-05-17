using System.Web.Mvc;

namespace University_Enrollment_System.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(string UserIdOrEmail, string Password)
        {
            // TODO: Add your authentication logic here
            if (UserIdOrEmail != "admin" || Password != "password") // Example check
            {
                ViewBag.Error = "Invalid User ID/Email or Password";
                return View();
            }

            // If login is successful, redirect to a dashboard or home page
            return RedirectToAction("Index", "Main");
        }
    }
}