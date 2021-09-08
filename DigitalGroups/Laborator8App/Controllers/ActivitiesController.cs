using System;
using Laborator5App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laborator8App.Models;
using Microsoft.AspNet.Identity;

namespace DigitalGroups.Controllers
{
    public class ActivitiesController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Activity a = db.Activities.Find(id);
            
            if (a.UserId == User.Identity.GetUserId() || User.IsInRole("Admin") || User.IsInRole("Editor"))
            {         
                db.Activities.Remove(a);
                db.SaveChanges();
                return Redirect("/Groups/Calendar/" + a.Id);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return Redirect("/Groups/Calendar/" + a.Id);
            }
        }

        public ActionResult New()
        {
            Activity app = new Activity();
            return View(app);
        }

        [HttpPost]
        public ActionResult New(Activity a)
        {
            DateTime Data = a.Date;
            var parsareData = Data.ToString("yyyy");
            Console.WriteLine(parsareData);
            a.UserId = User.Identity.GetUserId();
            try
            {
                db.Activities.Add(a);
                db.SaveChanges();
                return Redirect("/Groups/Calendar/" + a.Id);
            }

            catch (Exception e)
            {
                return Redirect("/Groups/Calendar/" + a.Id);
            }

        }

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Activity a = db.Activities.Find(id);

            if (a.UserId == User.Identity.GetUserId() || User.IsInRole("Admin") || User.IsInRole("Editor"))
            {
                return View(a);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Groups");
            }
        }
        
        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id, Activity requestActivity)
        {
            try
            {
                Activity a = db.Activities.Find(id);
                if (a.UserId == User.Identity.GetUserId() || User.IsInRole("Admin") || User.IsInRole("Editor"))
                {
                    if (TryUpdateModel(a))
                    {
                        a.ActivityName = requestActivity.ActivityName;
                        a.ActivityDescription = requestActivity.ActivityDescription;
                        a.Date = requestActivity.Date;                
                        db.SaveChanges();
                    }
                    return Redirect("/Groups/Show/" + a.Id);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                    return RedirectToAction("Index", "Groups");
                }
            }
            catch (Exception e)
            {
                return View(requestActivity);
            }

        }


       


    }
}