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
            if (Request.Cookies[Sys.LoginToken] != null)
                return true;

            return false;
        }

        // GET: Member
        public ActionResult Index()
        {
            if(!isLogged())
                return RedirectToAction("Index", "Index");


            using (entities = new wwbEntities())
            {
                //get token
                tokenCookie = Request.Cookies[Sys.LoginToken].Value.ToString();

                //get user
                user = entities.Users.Where(u => u.login_token == tokenCookie)
                                            .FirstOrDefault();

                //check user
                if (user == null) return RedirectToAction("Index", "Index");

                //check token
                if (user.login_token == null) return RedirectToAction("Index", "Index");

      

            }

            ViewData["mail"] = user.mail;


            return View();
        }


        public ActionResult Logout()
        {

            if (Request.Cookies[Sys.LoginToken] != null)
            {
                var c = new HttpCookie(Sys.LoginToken);
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }

            return RedirectToAction("Index", "Index");
        }
    }
}