using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorldWordBox.Models;

namespace WorldWordBox.Controllers
{
    public class IndexController : Controller
    {

        wwbEntities entities = new wwbEntities();


        // GET: Index
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Register(string userName,string password,string mail)
        {
            Users user = new Users();
            user.user_name = userName;
            user.password = password;
            user.mail = mail;
            user.create_date = DateTime.Now;

            entities.Users.Add(user);

            entities.SaveChanges();


            return Json(0, JsonRequestBehavior.AllowGet);
        }
    }
}