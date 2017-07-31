using System;
using System.Web;
using System.Web.Mvc;
using WorldWordBox.Models;
using System.Linq;
using System.Net;
using System.IO;

namespace WorldWordBox.Controllers
{

    public enum GroupAddingStatus
    {
        Success,
        Fail,
        AlreadyExist
    }

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

        public ActionResult AddWord()
        {
            if (!isLogged())
                return RedirectToAction("Index", "Index");

            ViewData["mail"] = Session["mail"];

            return View();
        }

        public ActionResult AddGroup()
        {
            if (!isLogged())
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddGroup(string groupName)
        {
            Groups group;
            UserGroups userGroup;
            int userId = Convert.ToInt32(Session[Sys.userId]); 
            try
            {
                using (entities = new wwbEntities())
                {
                    /* If group not exist then insert then just take id below */
                    group = entities.Groups
                           .Where(g => g.group_name == groupName)
                           .FirstOrDefault();

                    if (group == null)
                    {
                        group = new Groups();
                        group.group_name = groupName;

                        entities.Groups.Add(group);
                        entities.SaveChanges();
                    }
                }
                using (entities = new wwbEntities())
                {
                    /* Already exists */
                    userGroup = entities.UserGroups
                                .Where(ug => (ug.user_id == userId && ug.group_id == group.group_id))
                                .FirstOrDefault();

                    if (userGroup != null)
                    {
                        return Json(GroupAddingStatus.AlreadyExist, JsonRequestBehavior.AllowGet);
                    }

                    userGroup = new UserGroups();
                    userGroup.group_id = group.group_id;
                    userGroup.user_id = userId;

                    entities.UserGroups.Add(userGroup);
                    entities.SaveChanges();
                }
               
            }
            catch (Exception e)
            {
                return Json(GroupAddingStatus.Fail, JsonRequestBehavior.AllowGet);
            }
            

                return Json(GroupAddingStatus.Success, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Translate(string word, string language)
        {
            if (word.Equals(""))
                return Json("", JsonRequestBehavior.AllowGet);

            var url = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=" + Sys.yandexTranslateApiKey + "&text=" + word + "&lang=" + language + "&format=html";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            String line="",responseJson  = "";

            while ((line = reader.ReadLine()) != null)
                responseJson += line;

      

            return Json(responseJson, JsonRequestBehavior.AllowGet);
        }
    }
}