using System;
using System.Web;
using System.Web.Mvc;
using WorldWordBox.Models;
using System.Linq;

namespace WorldWordBox.Controllers
{
    public class MemberController : Controller
    {
        wwbEntities entities;
        Users user;
        string tokenCookie;

        public bool isLogged()
        {
            if (Session[Sys.userId] != null)
                return true;

            return false;
        }

        // GET: Member
        public ActionResult Index()
        {
            if(!isLogged())
                return RedirectToAction("Index", "Index");


            ViewData["mail"] = Session["mail"];


            return View();
        }


        public ActionResult Logout()
        {

            if (isLogged())
            {
                Session[Sys.userId] = null;
            }

            return RedirectToAction("Index", "Index");
        }
    }
}