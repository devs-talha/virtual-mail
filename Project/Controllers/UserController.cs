using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class UserController : Controller
    {
        private UserBLO userBLO = new UserBLO();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Login()
        {
            if (ValidateSession())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult Login(IFormCollection formCollection)
        {
            if (ValidateSession())
                return RedirectToAction("Index", "Home");

            string email = formCollection["email"].ToString();
            string password = formCollection["password"].ToString();

            try
            {
                User user = userBLO.Login(email, password);
                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(user));
            } catch (Exception e)
            {
                TempData["email"] = email;
                TempData["password"] = password;
                TempData["error"] = e.Message;
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Register()
        {
            if (ValidateSession())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult Register(IFormCollection formCollection)
        {
            if (ValidateSession())
                return RedirectToAction("Index", "Home");

            string firstName = formCollection["firstName"].ToString();
            string lastName = formCollection["lastName"].ToString();
            DateTime dateOfBirth = DateTime.Parse(formCollection["dateOfBirth"].ToString());
            string email = formCollection["email"].ToString();
            string password = formCollection["password"].ToString();
            string confirmPassword = formCollection["confirmPassword"].ToString();

            try
            {
                userBLO.Register(firstName, lastName, dateOfBirth, email, password, confirmPassword);
            }
            catch (Exception e)
            {
                TempData["firstName"] = firstName;
                TempData["lastName"] = lastName;
                TempData["dateOfBirth"] = dateOfBirth;
                TempData["email"] = email;
                TempData["password"] = password;
                TempData["confirmPassword"] = confirmPassword;
                TempData["error"] = e.Message;
                return View();
            }

            TempData["success"] = "Registered successfully";
            return View(); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult Logout()
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            HttpContext.Session.Remove("user");
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Update()
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            if (!HttpContext.Session.Keys.Contains("user"))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult Update(IFormCollection formCollection)
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            string firstName = formCollection["firstName"].ToString();
            string lastName = formCollection["lastName"].ToString();
            DateTime dateOfBirth = DateTime.Parse(formCollection["dateOfBirth"].ToString());
            string password = formCollection["password"].ToString();
            try
            {
                User updatedUser = userBLO.Update(firstName, lastName, dateOfBirth, password, 
                    JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user")));
                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(updatedUser));
            }
            catch (Exception e)
            {
                TempData["firstName"] = firstName;
                TempData["lastName"] = lastName;
                TempData["dateOfBirth"] = dateOfBirth;
                TempData["password"] = password;
                TempData["error"] = e.Message;
                return View();
            }

            TempData["success"] = "Updated successfully";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult Delete()
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            try
            {
                userBLO.Delete(JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user")));
                HttpContext.Session.Remove("user");
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Update");
            }

            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [NonAction]
        private bool ValidateSession()
        {
            return HttpContext.Session.Keys.Contains("user");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult RedirectToLogin()
        {
            TempData["error"] = "You must be logged in";
            return RedirectToAction("Login", "User");
        }
    }
}
