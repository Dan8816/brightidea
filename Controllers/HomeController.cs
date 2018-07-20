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
    public class HomeController : Controller
    {
        private YourContext _context;

        public HomeController(YourContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("HomePage")]
        public IActionResult Dashboard()
        {
            System.Console.WriteLine("**********Hitting the Dashboard route in HomeController**********");
            if(HttpContext.Session.GetInt32("user_id") == null)//if user tried to hit the route through url without login or reg
            {
                return RedirectToAction("Index", "User");//no login or reg go back to the root index
            }
            int? user_id = HttpContext.Session.GetInt32("user_id");
            DashboardModel view = new DashboardModel()//brings context of all models namespaces and context through in inheritance
            {
                ninjas = new User(),//users pts to db tablename
                likes = new Like(),//comments pts to db tablename
                secrets = new Message(),//messages pts to db tablename
                MessageList = _context.secrets.Include(m => m.Creator).ThenInclude(n => n.likes).ThenInclude(o => o.Promoter).ToList()
            };
            List<Message> allSecrets = _context.secrets.Include(m => m.Creator).ToList();//instantiates list all messages from all creators
            List<Like> allLikes = _context.likes.Include(m => m.Promoter).ToList();
            //int? user_id = HttpContext.Session.GetInt32("user_id");
            User CurrentUser = _context.ninjas.SingleOrDefault(u => u.Id == user_id);
            User Currentuser = _context.ninjas
                                .Include(user => user.secrets)
                                .Where(user => user.Id == user_id).SingleOrDefault();
            ViewBag.User = Currentuser;
            ViewBag.Secrets = allSecrets;
            ViewBag.Like = allLikes;
            return View(view);
        }

        [HttpPost]
        [Route("HomePage/Post")]
        public IActionResult PostMessage(Message NewMessage)
        {
            System.Console.WriteLine("******Hitting the PostMessage Route******");
            if(ModelState.IsValid) {
                int? user_id = HttpContext.Session.GetInt32("user_id");
                User CurrentUser = _context.ninjas.SingleOrDefault(user => user.Id == user_id);
                NewMessage.Created_At = DateTime.Now;
                NewMessage.Updated_At = DateTime.Now;
                NewMessage.Creator = CurrentUser;
                System.Console.WriteLine(NewMessage);
                _context.Add(NewMessage);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            System.Console.WriteLine("**********Invalid Message Content***********");
            return RedirectToAction("Dashboard"); 
        }

        [HttpGet]
        [Route("Delete/{MessageId}")]
        public IActionResult Delete(int MessageId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)//if user tried to hit the route through url without login or reg
            {
                return RedirectToAction("Index", "User");//no login or reg go back to the root index
            }
            Message ThisMessage = _context.secrets//instantiated an instance of Message class called ThisMessage within context of secrets db table
                            .Where(w => w.Id == MessageId).SingleOrDefault();//where id == int MessageId
            _context.secrets.Remove(ThisMessage);//deletes ThisMessage instance
            _context.SaveChanges();//Actually deletes the db row
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("Like/{MessageId}")]
        public IActionResult RSVP(int MessageId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)//if user tried to hit the route through url without login or reg
            {
                return RedirectToAction("Index", "User");//no login or reg go back to the root index
            }
            int? user_id = HttpContext.Session.GetInt32("user_id");//instantiated an instance of Wedding class called ThisWedding
            User curruser = _context.ninjas.Where(u => u.Id == user_id).SingleOrDefault();
            Message ThisMessage = _context.secrets
                            .Include(w => w.likes)
                            .ThenInclude(g => g.Promoter)
                            .SingleOrDefault(w => w.Id == MessageId);
            Like newLike = new Like
            {
                PromoterId = curruser.Id,
                Promoter = curruser,
                LikedId = ThisMessage.Id,
                Liked = ThisMessage
            };
            ThisMessage.likes.Add(newLike);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("Undo/{MessageId}")]
        public IActionResult Undo(int MessageId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null) {
                return RedirectToAction("Index", "User");
            }
            int? user_id = HttpContext.Session.GetInt32("user_id");
            User curruser = _context.ninjas.Where(u => u.Id == user_id).SingleOrDefault();
            Message ThisMessage = _context.secrets
                            .Include(w => w.likes)
                            .ThenInclude(g => g.Promoter)
                            .SingleOrDefault(w => w.Id == MessageId);
            Like oldLike = _context.likes.Where(i => i.PromoterId == user_id).Where(i => i.LikedId == MessageId).SingleOrDefault();
            ThisMessage.likes.Remove(oldLike);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("WhoLiked/{MessageId}")]
        public IActionResult WhoLiked(int MessageId)
        {
            System.Console.WriteLine("**********Hit the WhoLikes Route**********");
            if(HttpContext.Session.GetInt32("user_id") == null)
            {
                return RedirectToAction("Index", "User");
            }
            DashboardModel view = new DashboardModel()
            {
                secrets = _context.secrets.SingleOrDefault(c => c.Id == MessageId),
                likes = new Like(),
                NinjaList = _context.ninjas.Include(p => p.likes).ThenInclude(g => g.Promoter).ToList()
            };
            int? user_id = HttpContext.Session.GetInt32("user_id");
            User curruser = _context.ninjas.Where(u => u.Id == user_id).SingleOrDefault();
            ViewBag.User = curruser;
            //ViewBag.Messages = allSecrets.OrderByDescending(secret => secret.likes.Count);
            return View("WhoLiked", view);
        }

        [HttpGet]
        [Route("HomePage/{Id}")]
        public IActionResult HomePage(int Id)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)
            {
                return RedirectToAction("Index", "User");
            }
            int? user_id = HttpContext.Session.GetInt32("user_id");
            System.Console.WriteLine("**********Hitting the HomePage Route and the CreatorId is " + Id + "**********");
            DashboardModel view = new DashboardModel()
            {
                ninjas = _context.ninjas.SingleOrDefault(u => u.Id == Id),
                NinjaList = _context.ninjas.Include(p => p.likes).ThenInclude(y => y.Promoter).Where(r => r.Id == Id).ToList()          
            };
            List<Message> UserSecrets = _context.secrets.Where(c => c.CreatorId == Id).ToList();                         
            ViewBag.Message = UserSecrets;
            return View(view);

        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
