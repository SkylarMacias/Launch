using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BeltExam.Models;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }


        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("User.Email", "Email already in use.");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Add(user);
                    dbContext.SaveChanges();
                    int LoggedInUserId = user.UserId;
                    HttpContext.Session.SetInt32("LoggedInUserId", LoggedInUserId);
                    return RedirectToAction("BrightIdeas");
                } 
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser LoginUser)
        {
            if(ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == LoginUser.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginUser.Email", "Invalid Email/Password");
                    return View("Index");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(LoginUser, userInDb.Password, LoginUser.Password);

                if(result == 0)
                {
                    ModelState.AddModelError("LoginUser.Email", "Invalid Email/Password");
                    return View("Index");
                }
                int LoggedInUserId = userInDb.UserId;
                HttpContext.Session.SetInt32("LoggedInUserId", LoggedInUserId);
                return RedirectToAction("BrightIdeas");
            }
            else
            {
                return View("Index");
            }
        }


        [HttpGet("BrightIdeas")]
        public IActionResult BrightIdeas()
        {
                int? LoggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");
                if(LoggedInUserId != null)
                {
                    ViewModel ViewData = new ViewModel()
                    {
                        User = dbContext.Users.FirstOrDefault(u => u.UserId == LoggedInUserId),
                        DisplayIdeas = dbContext.Ideas
                        .Include(idea => idea.Creator)
                        .OrderByDescending(order => order.UsersWhoLiked.Count)
                        .Include(idea => idea.UsersWhoLiked)
                        .ThenInclude(like => like.User)
                        .ToList()
                    };
                return View(ViewData);
                }
                else
                {
                    return View("Index");
                }
        }

        [HttpPost("PostIdea")]
        public IActionResult PostIdea(Idea Idea)
        {
            if(ModelState.IsValid)
            {
                dbContext.Add(Idea);
                dbContext.SaveChanges();
                return RedirectToAction("BrightIdeas");
            }
            else
            {
                int? LoggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");
                ViewModel ViewData = new ViewModel()
                {
                    User = dbContext.Users.FirstOrDefault(u => u.UserId == LoggedInUserId),
                    DisplayIdeas = dbContext.Ideas
                    .Include(idea => idea.Creator)
                    .Include(idea => idea.UsersWhoLiked)
                    .ThenInclude(like => like.User)
                    .ToList()
                };
                return View("BrightIdeas", ViewData);
            }
        }


        [HttpPost("LikeThisIdea")]
        public IActionResult LikeThisIdea(Like Like)
        {
                dbContext.Add(Like);
                dbContext.SaveChanges();
                return RedirectToAction("BrightIdeas");
        }
        
        
        [HttpGet("UserPage/{id}")]
        public IActionResult UserPage(int id)
        {
                ViewModel ViewData = new ViewModel()
                {
                    User = dbContext.Users
                    .Include(user => user.CreatedIdeas)
                    .FirstOrDefault(u => u.UserId == id),
                    DisplayLikes = dbContext.Likes
                    .Include(idea => idea.Idea)
                    .ThenInclude(user => user.Creator)
                    .Where(u => u.Idea.Creator.UserId == id)
                    .ToList()
                };
                return View(ViewData);
        }
        [HttpGet("BrightIdeas/{id}")]
        public IActionResult IdeaPage(int id)
        {
                int? LoggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");
                ViewModel ViewData = new ViewModel()
                {
                    User = dbContext.Users.FirstOrDefault(u => u.UserId == LoggedInUserId),
                    Idea = dbContext.Ideas
                    .Include(idea => idea.Creator)
                    .Include(like => like.UsersWhoLiked)
                    .ThenInclude(user => user.User)
                    .FirstOrDefault(i => i.IdeaId == id)
                };
                return View(ViewData);
        }



        [HttpGet("delete/{id}")]
        public IActionResult DeleteIdea(int id)
        {
            Idea RetrievedIdea = dbContext.Ideas.FirstOrDefault(i => i.IdeaId == id);
            if(RetrievedIdea.UserId == HttpContext.Session.GetInt32("LoggedInUserId"))
            {
                dbContext.Ideas.Remove(RetrievedIdea);
                dbContext.SaveChanges();
            }
            return RedirectToAction("BrightIdeas");
            

        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
