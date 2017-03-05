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
        LoginTokens token;
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
                //check token
                token = new LoginTokens();
                tokenCookie = Request.Cookies[Sys.LoginToken].Value.ToString();
                token = entities.LoginTokens.Where(t => t.token == tokenCookie)
                                            .FirstOrDefault();

                if(token == null) return RedirectToAction("Index", "Index");

                //check user
                user = entities.Users.Where(u => u.user_id == token.user_id)
                                     .FirstOrDefault();

                if(user == null) return RedirectToAction("Index", "Index");

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