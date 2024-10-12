using Microsoft.AspNetCore.Mvc;
using OnlineGroceryApps.Models;
using System.Text;
using System.Security.Cryptography;
using DNTCaptcha.Core.Providers;
using DNTCaptcha.Core;

namespace OnlineGroceryApps.Controllers
{
    public class AdminController : Controller
    {

        public readonly IDNTCaptchaValidatorService _validatorService;
        public AdminController(IDNTCaptchaValidatorService validatorService)
        {
            _validatorService = validatorService;
        }

        GroceryDbContext dc = new GroceryDbContext(); 
        public IActionResult Home()
        {
            return View();
        }

      


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDNTCaptcha(ErrorMessage = "Please Enter Valid Captcha",
            CaptchaGeneratorLanguage = Language.English,
            CaptchaGeneratorDisplayMode = DisplayMode.ShowDigits)]

        public IActionResult Login(string t1, string Password)
        {

            if (ModelState.IsValid)
            {
                if (!_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.ShowDigits))
                {
                    this.ModelState.AddModelError(DNTCaptchaTagHelper.CaptchaInputName, "Please Enter Valid Captcha.");
                }

                //  var r = dv.ApplicantRegistrations.Where(c => c.Username == t1).FirstOrDefault();

                //  string st = Encoding.UTF8.GetString(r.Password);

                var res = (from t in dc.Admins
                           where t.AdminName == t1
                           select t).Count();

                if (res > 0)
                {

                    HttpContext.Session.SetString("uid", t1);

                    //code to navigate
                    return RedirectToAction("Home");

                }
                else
                {
                    ViewData["r"] = "Invalid User..!!!!";
                }
            }
            return View();
        }
         private byte[] HasPassword(string password)
        {
            var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }




        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDNTCaptcha(ErrorMessage = "Please Enter Valid Captcha",
            CaptchaGeneratorLanguage = Language.English,
            CaptchaGeneratorDisplayMode = DisplayMode.ShowDigits)]

        public IActionResult Register(Admin u)
        {

            if (ModelState.IsValid)
            {
                if (!_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.ShowDigits))
                {
                    this.ModelState.AddModelError(DNTCaptchaTagHelper.CaptchaInputName, "Please Enter Valid Captcha.");
                }

                // byte[] password = Encoding.ASCII.GetBytes(u.Password);
                //u.Password = password;
                // string truncatedPassword = AdminPassword.Substring(0, Math.Min(AdminPassword.Length, 10));

                //u.AdminPassword= Encoding.UTF8.GetBytes(truncatedPassword);

                u.AdminPassword = HasPassword(u.NormPassword);
                dc.Admins.Add(u);
                int i = dc.SaveChanges();
                if (i > 0)
                {
                    ViewData["v"] = "User created successfully";

                }
               

            }
            return View();
        }

    }
}
