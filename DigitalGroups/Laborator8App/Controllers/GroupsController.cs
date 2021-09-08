using Laborator5App.Models;
using Laborator8App.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace DigitalGroups.Controllers
{
    public class GroupsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Groups
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Index(string id)
        {
            //var groups = db.Groups.Include("Category");
            string idUser = User.Identity.GetUserId();

            var allGroups = from w in db.Groups
                            select w;

            var groups = from v in db.Groups
                         join a in db.GroupUsers on v.Id equals a.Id
                         join b in db.Users on a.ApplicationUsers_Id equals b.Id
                         where b.Id == idUser
                         where a.GroupRoleId != "4"
                         select v;

            var x = allGroups.Except(groups);

            switch (id)
            {
                case "A-Z":
                    x = x.OrderBy(s => s.Name);
                    break;
                case "Z-A":
                    x = x.OrderByDescending(s => s.Name);
                    break;
                case "NEW-OLD":
                    x = x.OrderByDescending(s => s.Date);
                    break;
                case "OLD-NEW":
                    x = x.OrderBy(s => s.Date);
                    break;
                /*case "MOST-POPULAR":
                    x = ...
                    break;
                case "LEAST-POPULAR":
                    x = ...
                    break;*/
                default:
                    x = x.OrderBy(s => s.Name);
                    break;

            }

            ViewBag.Groups = x;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // --------------------------------------

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult AllMyGroups()
        {
            //var groups = db.Groups.Include("Category");
            string idUser = User.Identity.GetUserId();

            var x = from v in db.Groups
                    join a in db.GroupUsers on v.Id equals a.Id
                    join b in db.Users on a.ApplicationUsers_Id equals b.Id
                    where b.Id == idUser
                    where a.GroupRoleId != "4"
                    select v;

            ViewBag.Groups = x;


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // ---------------------------------------


        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Requests(int id)
        {
            //var groups = db.Groups.Include("Category");
            if (AccessAd(id) == true)
            {
                string idUser = User.Identity.GetUserId();

                var x = from a in db.Users
                        join v in db.GroupUsers on a.Id equals v.ApplicationUsers_Id
                        where v.GroupRoleId == "4"
                        where v.Id == id
                        select a;

                ViewBag.Users = x;
                ViewBag.idUser = idUser;
                ViewBag.id = id;

                if (TempData.ContainsKey("message"))
                {
                    ViewBag.Message = TempData["message"];
                }

                return View();
            }
            else
            {
                TempData["message"] = "Nu aveti acces!----------------";
                return RedirectToAction("AllMyGroups");
            }
        }




        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult MyGroups()
        {
            string idUser = User.Identity.GetUserId();
            var groups = from group1 in db.Groups
                         join category in db.Categories on group1.CategoryId equals category.CategoryId
                         where group1.UserId == idUser
                         select group1;

            ViewBag.Groups = groups;

            if (!groups.Any())
            {
                TempData["message"] = "Momentan nu administrezi niciun grup!";
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // -----------------------------------------

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Calendar(int id)
        {
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
            ViewBag.userId = User.Identity.GetUserId();
            if (AccessUs(id) == true)
            {
                Group group = db.Groups.Find(id);
                //SetAccessRights();
                return View(group);
            }
            else
            {
                TempData["message"] = "Nu sunteti in grup1";
                return RedirectToAction("Index");
            }

        }


        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(int id)
        {
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
            string idUser = User.Identity.GetUserId();
            ViewBag.idUser = idUser;
            ViewBag.ok = false;
            string userRole = "0";
            if (AccessUs(id) == true)  // Daca e in grup
            {
                ViewBag.inGroup = true;
                
                if ((gu == null || gu.GroupRoleId == "4") && (User.IsInRole("Admin") || User.IsInRole("Editor")))
                {
                     userRole = "1";
                     ViewBag.ok = true;

                }
                else
                {
                     userRole = gu.GroupRoleId;
                }
                
                
            }
            else    // Daca nu e in grup
            {
                if(gu != null )
                    if(gu.GroupRoleId == "4")
                    {
                        userRole = "4";

                    }
                else
                    ViewBag.inGroup = false;
            }

            ViewBag.groupRole = userRole;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            Group group = db.Groups.Find(id);
            var x = from user in db.Users
                    join a in db.GroupUsers on user.Id equals a.ApplicationUsers_Id
                    join b in db.Groups on a.Id equals b.Id
                    join c in db.GroupRoles on a.GroupRoleId equals c.GroupRoleId
                    where b.Id == id
                    where user.Id != idUser
                    where a.GroupRoleId != "4"
                    select new ViewModel_1
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        RoleName = c.GroupRoleName,
                        
                    };
                    
                   
            ViewBag.Users = x;

            //SetAccessRights();
            return View(group);

        }


        public ActionResult New()
        {
            Group group = new Group();
            group.Categ = GetAllCategories();
            group.UserId = User.Identity.GetUserId();

            return View(group);
        }


        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult New(Group group)
        {
            //group.Categ = GetAllCategories();
            group.Date = DateTime.Now;
            group.UserId = User.Identity.GetUserId();
            GroupUser gu = new GroupUser();
            gu.Id = group.Id;
            gu.ApplicationUsers_Id = group.UserId;
            gu.GroupRoleId = "1";
            try
            {
                if (ModelState.IsValid)
                {
                    db.Groups.Add(group);
                    db.GroupUsers.Add(gu);
                    db.SaveChanges();
                    TempData["message"] = "Grupul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    group.Categ = GetAllCategories();
                    return View(group);
                }
            }
            catch (Exception e)
            {
                group.Categ = GetAllCategories();
                return View(group);
            }
        }


        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Request(int id)
        {
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());

            if (gu == null) // Daca nu e in grup
            {
                GroupUser a = new GroupUser
                {

                    ApplicationUsers_Id = User.Identity.GetUserId(),
                    Id = id,
                    GroupRoleId = "4"
                };
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.GroupUsers.Add(a);
                        db.SaveChanges();
                        return Redirect("/Groups/Show/" + a.Id);
                    }

                    else
                    {
                        Group art = db.Groups.Find(a.Id);
                        return Redirect("/Groups/Show/" + a.Id);
                    }

                }

                catch (Exception e)
                {
                    Group art = db.Groups.Find(a.Id);
                    return Redirect("/Groups/Show/" + a.Id);
                }
            }
            else    // Daca e in grup
            {
                TempData["message"] = "Esti deja in grup!";
                return RedirectToAction("Index");
            }

        }


        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult AddUser(int id, string param)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    GroupUser gru = db.GroupUsers.Find(id, param);
                    GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
                    string userRole = gu.GroupRoleId;

                    if (AccessAd(id) == true)
                    {
                        if (TryUpdateModel(gru))
                        {
                            gru.GroupRoleId = "3";
                            db.SaveChanges();
                            TempData["message"] = "Utilizatorul a fost acceptat in grup!";
                        }
                        return Redirect("/Groups/Requests/" + id);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine!";
                        return Redirect("/Groups/Show/" + id);
                    }
                }
                else
                {
                    return Redirect("/Groups/Show/" + id);
                }
            }
            catch (Exception e)
            {
                return Redirect("/Groups/Show/" + id);
            }
        }


        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id)
        {
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
            if (AccessUs(id) == true)
            {
               /* string userRole = "0";
                if (gu == null)
                {
                    userRole = "1";
                }
                else
                {
                    userRole = gu.GroupRoleId;
                }*/

                Group group = db.Groups.Find(id);
                group.Categ = GetAllCategories();

                if (AccessEd(id))
                {
                    return View(group);
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                return RedirectToAction("Index");
            }
        }


        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult EditUser(int id, string param)
        {
            GroupUser gu = db.GroupUsers.Find(id, param);
            ViewBag.id = gu.Id;
            ViewBag.param = gu.ApplicationUsers_Id;
            ViewBag.rol = gu.GroupRoleId;
            gu.Roles = GetAllRoles();
            string userRole = User.Identity.GetUserId();
            if (AccessAd(id) == true)
            {
                return View(gu);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                return RedirectToAction("Index");
            }

            /*if (gu != null)
            {
                string userRole = User.Identity.GetUserId();

                var roleName = from user in db.GroupUsers
                        join a in db.GroupRoles on user.GroupRoleId equals a.GroupRoleId
                        where user.Id == id
                        select a.GroupRoleName;

                //Group group = db.Groups.Find(id);
                // group.Categ = GetAllCategories();
                gu.Roles = GetAllRoles();


            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine-----------";
                return RedirectToAction("Index");
            }*/
        }


        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult EditUser(int id, string param, GroupUser rqGroupUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    

                    if (AccessEd(id) == true)
                    {
                        string userRole;
                        GroupUser gru = db.GroupUsers.Find(id, param);
                        GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
                        if(gu == null && User.IsInRole("Admin"))
                        {
                            userRole = "1";
                        }
                        else
                        {
                            userRole = gu.GroupRoleId;
                        }
                        

                        if (TryUpdateModel(gru))
                        {
                            gru.Id = rqGroupUser.Id;
                            gru.ApplicationUsers_Id = rqGroupUser.ApplicationUsers_Id;
                            gru.GroupRoleId = rqGroupUser.GroupRoleId;
                            db.SaveChanges();
                            TempData["message"] = "Rolul utilizatorului a fost modificat!";
                        }
                        return Redirect("/Groups/Show/" + id);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine, sau in care nu sunteti editor!";
                        return Redirect("/Groups/Show/" + id);
                    }
                }
                else
                {
                    rqGroupUser.Roles = GetAllRoles();
                    return View(rqGroupUser);
                }
            }
            catch (Exception e)
            {
                rqGroupUser.Roles = GetAllRoles();
                return View(rqGroupUser);
            }
        }


        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id, Group rqGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Group group = db.Groups.Find(id);
                    GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
                    string userRole = gu.GroupRoleId;

                    if (AccessEd(id) == true)
                    {
                        if (TryUpdateModel(group))
                        {
                            group.Name = rqGroup.Name;
                            group.Description = rqGroup.Description;
                            group.CategoryId = rqGroup.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Grupul a fost modificat";
                        }
                        return Redirect("/Groups/Show/" + group.Id);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine, sau in care nu sunteti editor!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    rqGroup.Categ = GetAllCategories();
                    return View(rqGroup);
                }
            }
            catch (Exception e)
            {
                rqGroup.Categ = GetAllCategories();
                return View(rqGroup);
            }
        }


        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
            string userRole = gu.GroupRoleId;


            Group group = db.Groups.Find(id);

            if (AccessAd(id))
            {
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost sters";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un grupul care nu va apartine";
                return RedirectToAction("Index");
            }
        }


        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Leave(int id)
        {
            Group group = db.Groups.Find(id);
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
            string userRole = gu.GroupRoleId;
            ViewBag.groupRole = userRole;
            if (AccessUs(id))
            {
                if ((gu == null || gu.GroupRoleId == "4") && (User.IsInRole("Admin") || User.IsInRole("Editor")))
                {
                    TempData["message"] = "Ai parasit grupul " + group.Name;
                    return RedirectToAction("Index");
                }
                else
                {
                    db.GroupUsers.Remove(gu);
                    db.SaveChanges();
                    TempData["message"] = "Ai parasit grupul " + group.Name;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["message"] = "Nu esti in grup!";
                return RedirectToAction("Index");
            }


        }


        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Remove(int id, string param)
        {
            Group group = db.Groups.Find(id);
            GroupUser gu = db.GroupUsers.Find(id, param);
            string userRole = gu.GroupRoleId;
            ViewBag.groupRole = userRole;
            if (AccessUs(id))
            {
                db.GroupUsers.Remove(gu);
                db.SaveChanges();
                TempData["message"] = "Utilizatorul a fost exclus din grup " + group.Name;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu esti in grup!";
                return RedirectToAction("Index");
            }

        }


        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult IgnoreRequest(int id, string param)
        {
            //Group group = db.Groups.Find(id);
            GroupUser gu = db.GroupUsers.Find(id, param);
            //string userRole = gu.GroupRoleId;
            //ViewBag.groupRole = userRole;
            if (AccessUs(id) || (gu != null && gu.GroupRoleId == "4" && gu.ApplicationUsers_Id == User.Identity.GetUserId() ))
            {
                db.GroupUsers.Remove(gu);
                db.SaveChanges();
                //TempData["message"] = "Ai parasit grupul " + group.Name;
                return Redirect("/Groups/Show/" + id);
            }
            else
            {
                TempData["message"] = "Nu exista cererea!";
                return Redirect("/Groups/Show/" + id);
            }

        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            return selectList;
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.GroupRoles
                        where role.GroupRoleName != "Request"
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.GroupRoleId.ToString(),
                    Text = role.GroupRoleName.ToString()
                });
            }

            return selectList;
        }


        /*private void SetAccessRights()
        {
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Editor") || User.IsInRole("Admin") || User.IsInRole("User"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }*/

        private bool AccessAd(int id)
        {
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Admin") || (gu != null && gu.GroupRoleId == "1"))
            {
                ViewBag.afisareButoane = true;
                return true;
            }
            else
            {
                return false;
            }

        }


        private bool AccessEd(int id)
        {
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Admin") || User.IsInRole("Editor") || (gu != null && gu.GroupRoleId == "1") || (gu != null && gu.GroupRoleId == "2"))
            {
                ViewBag.afisareButoane = true;
                return true;
            }
            else
            {
                return false;
            }

        }


        private bool AccessUs(int id)
        {
            GroupUser gu = db.GroupUsers.Find(id, User.Identity.GetUserId());
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Admin") || User.IsInRole("Editor") || (gu != null && gu.GroupRoleId != "4"))
            {
                ViewBag.afisareButoane = true;
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
