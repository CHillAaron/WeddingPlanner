using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WeddingPlanner.Context;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public User UserInDb()
        {
            return _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User reg)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == reg.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                reg.Password = Hasher.HashPassword(reg, reg.Password);
                _context.Users.Add(reg);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", reg.UserId);
                return RedirectToAction("Success");
            }
            else
            {
                return View("Index");
            }
        }
        [HttpPost("Login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    return View("Index");

                }

                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Success");
            }
            else
            {
                return View("Index");
            }
        }
        [HttpGet("Success")]
        public IActionResult Success()
        {
            User userInDb = UserInDb();
            if (userInDb == null)
            {
                return RedirectToAction("LogOut");
            }
            ViewBag.User = userInDb;
            List<Event> AllEvents = _context.Events
                                            .Include(u => u.Creator)
                                            .Include(u => u.GuestList)
                                            .ThenInclude(r => r.Guest)
                                            .ToList();
            return View(AllEvents);
        }
        [HttpGet("New/NewEvent")]
        public IActionResult NewEvent()
        {
            User userInDb = UserInDb();
            if (userInDb == null)
            {
                return RedirectToAction("LogOut");
            }
            ViewBag.User = userInDb;
            return View();
        }
        [HttpPost("create/NewEvent")]
        public IActionResult CreateEvent(Event wedding)
        {
            User userInDb = UserInDb();
            if (userInDb == null)
            {
                return RedirectToAction("LogOut");
            }
            if (ModelState.IsValid)
            {
                _context.Events.Add(wedding);
                _context.SaveChanges();
                return Redirect($"/EventDetail/{wedding.EventId}");
            }
            else
            {
                ViewBag.User = userInDb;
                return View("NewEvent");
            }
        }

        [HttpGet("EventDetail/{EventId}")]
        public IActionResult ShowEvent(int eventId)
        {
            User userInDb = UserInDb();
            if (userInDb == null)
            {
                return RedirectToAction("LogOut");
            }

            Event show = _context.Events
                                 .Include(u => u.Creator)
                                 .Include(u => u.GuestList)
                                 .ThenInclude(r => r.Guest)
                                 .FirstOrDefault(u => u.EventId == eventId);
            ViewBag.User = userInDb;
            return View(show);
        }

        [HttpGet("{status}/{EventId}/{UserId}")]
        public IActionResult RsvpResponse(string status, int eventId, int userId)
        {
            User userInDb = UserInDb();
            if (userInDb == null)
            {
                return RedirectToAction("LogOut");
            }
            if (status == "Join")
            {
                RSVp going = new RSVp();
                going.UserId = userId;
                going.EventId = eventId;
                _context.Rsvps.Add(going);
                _context.SaveChanges();
                
            }else{
                RSVp leave = _context.Rsvps.FirstOrDefault(r => r.EventId == eventId && r.UserId == userId);
                _context.Rsvps.Remove(leave);
                _context.SaveChanges();
            }
            return RedirectToAction("Success");
        }
        [HttpGet("Delete/{EventId}")]
        public IActionResult DeleteEvent(int eventId)
        {
            Event delete = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            _context.Events.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction("Success");
        }


        [HttpGet("LogOut")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
