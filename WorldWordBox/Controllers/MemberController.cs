using System;
using System.Web;
using System.Web.Mvc;
using WorldWordBox.Models;
using System.Linq;
using System.Net;
using System.IO;

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

        public ActionResult AddWord()
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