using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using beltExamActivity.Models;
using Microsoft.EntityFrameworkCore;

namespace beltExamActivity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BeltExamContext db;

        public HomeController(BeltExamContext context)
        {
            db = context;
        }

        private int? uid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserId");
            }
        }

        private bool isLoggedIn
        {
            get
            {
                return uid != null;
            }
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            if (isLoggedIn)
            {
                return RedirectToAction("LoggedIn");
            }
            return View("Index");
        }

        [HttpPost("/Register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Is taken");
                }
            }
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);

            db.Users.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("FirstName", newUser.FirstName);
            return RedirectToAction("All");

        }

        [HttpPost("/Login")]
        public IActionResult Login(LoginUser loginUser)
        {
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }

            User dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);

            if (dbUser == null)
            {
                ModelState.AddModelError("LoginError", "Email not found.");
                return View("Index");
            }
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            var pwCompareResult = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

            if (pwCompareResult == 0)
            {
                ModelState.AddModelError("LoginPassword", "Invalid Password.");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            HttpContext.Session.SetString("FirstName", dbUser.FirstName);
            return RedirectToAction("All");
        }

        [HttpPost("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("/All")]
        public IActionResult All()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            List<BeltExam> allActvities = db.Activity.ToList();

            foreach (var activity in allActvities)
            {
                if (activity.ActivityDate <= DateTime.Now)
                {
                    db.Activity.Remove(activity);
                    db.SaveChanges();
                }
            }

            //Activity.Models.Activity is because my project and namespace is the same name (Don't do this in the future.)
            List<BeltExam> newActivities = db.Activity
            .OrderBy(date => date.ActivityDate)
            .Include(c => c.ActiveUser)
            .ThenInclude(user => user.User)
            .ToList();
            return View("All", newActivities);
        }

        [HttpGet("/NewActivity")]
        public IActionResult CreateActivity()
        {
            return View("NewActivity");
        }

        [HttpPost("/NewActivity")]
        public IActionResult NewActivity(BeltExam newActivity)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid == false)
            {
                Console.WriteLine("Fail");
                return View("NewActivity");
            }
            if (newActivity.ActivityDate < DateTime.Now)
            {
                ModelState.AddModelError("Date", "Must be in the future!");
                return View("NewActivity");
            }

            User NewActivityCreater = db.Users.FirstOrDefault(u => u.UserId == (int)uid);
            newActivity.User = NewActivityCreater;
            db.Add(newActivity);
            db.SaveChanges();
            return RedirectToAction("All");
        }

        [HttpPost("/Activity/{activityId}/Join")]
        public IActionResult Join(int ActivityId)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            ActivityUser ActiveUser = db.ActiveUser
            .FirstOrDefault(r => r.UserId == (int)uid && r.ActivityId == ActivityId);

            if (ActiveUser == null)
            {
                ActivityUser activeUser = new ActivityUser()
                {
                    ActivityId = ActivityId,
                    UserId = (int)uid
                };
                db.ActiveUser.Add(activeUser);
            }
            else
            {
                db.ActiveUser.Remove(ActiveUser);
            }

            db.SaveChanges();
            return RedirectToAction("All");
        }

        [HttpPost("Delete/{activityId}/Delete")]
        public IActionResult Delete(int ActivityId)
        {
            BeltExam Activity = db.Activity.FirstOrDefault(w => w.ActivityId == ActivityId);

            if (Activity == null)
            {
                return RedirectToAction("All");
            }

            db.Activity.Remove(Activity);
            db.SaveChanges();
            return RedirectToAction("All");
        }

        [HttpGet("/ActivityDetails/{activityId}")]
        public IActionResult ActivityDetails(int ActivityId)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            BeltExam Activity = db.Activity
            .Include(activ => activ.ActiveUser)
            .ThenInclude(ActiveUser => ActiveUser.User)
            .Include(activ => activ.User)
            .FirstOrDefault(w => w.ActivityId == ActivityId);
            Console.WriteLine(Activity);

            if (Activity == null)
            {
                return RedirectToAction("All");
            }

            return View("ActivityDetails", Activity);

        }

        [HttpGet("/Activity/{activityId}/edit")]
        public IActionResult Edit(int activityId)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            BeltExam Activity = db.Activity.FirstOrDefault(w => w.ActivityId == activityId);

            // The edit button will be hidden if you are not the author,
            // but the user could still type the URL in manually, so
            // prevent them from editing if they are not the author.
            if (Activity == null || Activity.UserId != uid)
            {
                return RedirectToAction("All");
            }

            return View("Edit", Activity);
        }

        [HttpPost("/Activity/{activityId}/update")]
        public IActionResult Update(int activityId, BeltExam editedActivity)
        {
            if (ModelState.IsValid == false)
            {
                editedActivity.ActivityId = activityId;
                // Send back to the page with the current form edited data to
                // display errors.
                return View("Edit", editedActivity);
            }

            BeltExam dbActivity = db.Activity.FirstOrDefault(w => w.ActivityId == activityId);

            if (dbActivity == null)
            {
                return RedirectToAction("All");
            }

            dbActivity.ActivityName = editedActivity.ActivityName;
            dbActivity.Duration = editedActivity.Duration;
            dbActivity.ActivityDate = editedActivity.ActivityDate;
            dbActivity.UpdatedAt = DateTime.Now;

            db.Activity.Update(dbActivity);
            db.SaveChanges();

            /* 
            When redirecting to action that has params, you need to pass in a
            dict with keys that match param names and the value of the keys are
            the values for the params.
            */
            return RedirectToAction("Details", new { ActivityId = activityId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
