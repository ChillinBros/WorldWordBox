using System;
using System.Linq;
using System.Web.Mvc;
using WorldWordBox.Models;
using CryptoHelper;

namespace WorldWordBox.Controllers
{


    public enum RegisterStatus
    {
        Success,
        Fail,
        MailExist
    }

    public enum LoginStatus
    {
        Success,
        UserNotExist,
        IncorrectPassword,
        Fail,
    }

    public class IndexController : Controller
    {


        wwbEntities entities = new wwbEntities();


        public bool isLogged()
        {
            if (Request.Cookies[Sys.LoginToken] != null)
                return true;

            return false;
        }


        // GET: Index
        public ActionResult Index()
        {
            if(isLogged())
                return RedirectToAction("Index", "Member");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login(string mail,string password)
        {
            try
            {
                Users userControl = entities.Users
                           .Where(u => u.mail == mail)
                           .FirstOrDefault();

                if (userControl == null)
                    return Json(LoginStatus.UserNotExist, JsonRequestBehavior.DenyGet);

                if (!Crypto.VerifyHashedPassword(userControl.password, password))
                    return Json(LoginStatus.IncorrectPassword, JsonRequestBehavior.DenyGet);
                else
                {
                    //creating token
                    LoginTokens token = new LoginTokens();
                    token.token = Crypto.HashPassword(userControl.mail + userControl.password + userControl.create_date);
                    token.user_id = userControl.user_id;

                    //check if token already added
                    LoginTokens tokenControl = new LoginTokens();
                    tokenControl = entities.LoginTokens
                                            .Where(lt => lt.user_id == userControl.user_id)
                                            .FirstOrDefault();

                        if(tokenControl!=null)
                        {
                            tokenControl.token = token.token;
                            entities.SaveChanges();
                        }else
                        {
                            entities.LoginTokens.Add(token);
                            entities.SaveChanges();
                        }

                        
                    Response.Cookies[Sys.LoginToken].Value = token.token;
                    return Json(LoginStatus.Success, JsonRequestBehavior.DenyGet);
                }
                    
            }
            catch (Exception e)
            {
                return Json(LoginStatus.Fail, JsonRequestBehavior.DenyGet);
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Register(string mail,string password)
        {
            try
            {
                ////mail control
                Users userControl = entities.Users
                            .Where(u => u.mail == mail)
                            .FirstOrDefault();

            if(userControl != null)
                return Json(RegisterStatus.MailExist, JsonRequestBehavior.AllowGet);

            string hashPassword = Crypto.HashPassword(password);
           

            Users user = new Users();
            user.password = hashPassword;
            user.mail = mail;
            user.create_date = DateTime.Now;

            entities.Users.Add(user);

           
                entities.SaveChanges();
            }
            catch (Exception)
            {
                return Json(RegisterStatus.Fail, JsonRequestBehavior.AllowGet);
            }


            return Json(RegisterStatus.Success, JsonRequestBehavior.AllowGet);
        }
    }

}