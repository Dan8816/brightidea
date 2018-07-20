using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DojoSecret.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DojoSecret.Controllers
{
    
    public class UserController : Controller
    {
        private YourContext _context;
        public UserController(YourContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterUser(RegisterUser userReg)//this points to the RegisterUser model for error validation purposes they are broken up
        {
            System.Console.WriteLine("**********Hitting the register route in user controller**********");
            if(_context.ninjas.Where(checkUser => checkUser.Email == userReg.Email).FirstOrDefault() != null)
            {
                System.Console.WriteLine("**********The email was already registered in the database**********");
                ModelState.AddModelError("Email", "Email already in use");
            }
            System.Console.WriteLine("**********The email is unique, horray**********");
            PasswordHasher<RegisterUser> hasher = new PasswordHasher<RegisterUser>();//this hashes the form pw to an encrypted pw for the db
            if(ModelState.IsValid)//this bool reconciles the form data with the model validations pointing to the RegisterUser model
            {
                User newUser = new User//instantiates a new user and sets the values in the db fields to the following data
                {
                    Name = userReg.Name,
                    Alias = userReg.Alias,
                    Email = userReg.Email,
                    Password = hasher.HashPassword(userReg, userReg.Password),//this just sets the db pw value to the hash already generated
                    Created_At = DateTime.Now,
                    Updated_At = DateTime.Now,          
                };
                _context.Add(newUser);//added but not saves
                _context.SaveChanges();//at this point we have a new user with a unique email compared to db
                HttpContext.Session.SetInt32("user_id", newUser.Id);//sets the newUser id into session for use downstream
                int? user_id = newUser.Id;
                System.Console.WriteLine("**********" + user_id + "**********");//verifies the new user id
                return RedirectToAction ("Dashboard", "Home");//downstream to a new controller
            };
            System.Console.WriteLine("**********Registration failed**********");
            return View("Index");//falied to register
        }

        [Route("login")]
         public IActionResult LoginUser(LoginUser userLog)//this points to the LoginUser model for error validation purposes they are broken up
        {
            System.Console.WriteLine("**********Hitting the login route in user controller**********");
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();//this hashes the form pw to an encrypted pw for the db
            User loginUser = _context.ninjas.Where(ExistUser => ExistUser.Email == userLog.LogEmail).SingleOrDefault();
            if(loginUser == null)
            {
                ModelState.AddModelError("LogEmail", "Invalid Email/Password.");
            }
            else if(hasher.VerifyHashedPassword(userLog, loginUser.Password, userLog.LogPassword) == 0)
            {
                ModelState.AddModelError("LogEmail", "Invalid Email/Password.");
            }
            if(!ModelState.IsValid)
            {
                System.Console.WriteLine("**********Model state is not valid**********");
                return View("Index");
            }
            HttpContext.Session.SetInt32("user_id", loginUser.Id);
            System.Console.WriteLine("**********Login successful**********");
            return RedirectToAction ("Dashboard", "Home");
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["success"] = "You have successfully logged out.";
            return Redirect("/");
        }
    }
}