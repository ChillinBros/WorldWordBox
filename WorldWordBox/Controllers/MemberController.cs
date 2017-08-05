using System;
using System.Web;
using System.Web.Mvc;
using WorldWordBox.Models;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace WorldWordBox.Controllers
{

    public enum GroupAddingStatus
    {
        Success,
        Fail,
        AlreadyExist
    }

    public enum WordAddingStatus
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
            if (!isLogged())
                return RedirectToAction("Index", "Index");

            ViewData["mail"] = Session["mail"];


            return View();
        }

        public ActionResult AddWord()
        {
            if (!isLogged())
                return RedirectToAction("Index", "Index");

           

            List<Group> userGroupList = new List<Group>();
            List<UserGroups> userGroups = null;
            List<Groups> groups = null;
            Groups group = null;
            int userId = Convert.ToInt32(Session[Sys.userId]);


            using (entities = new wwbEntities())
            {
                userGroups = entities.UserGroups
                                    .Where(g => (g.user_id == userId))
                                    .ToList();

                string s = "";
                if (userGroups != null)
                {
                    foreach (UserGroups ug in userGroups)
                    {
                        group = entities.Groups
                                    .Where(g => g.group_id == ug.group_id)
                                    .FirstOrDefault();

                        userGroupList.Add(new Group(group.group_id, group.group_name));
                        s += group.group_name + " ";
                    }
                    ViewData[Sys.userGroups] = userGroupList;
                }

            }

            return View();

            return View();
        }

        public ActionResult AddGroup()
        {
            if (!isLogged())
                return RedirectToAction("Index", "Index");

            ViewData["mail"] = Session["mail"];

            List<Group> userGroupList = new List<Group>();
            List<UserGroups> userGroups = null;
            List<Groups> groups = null;
            Groups group = null;
            int userId = Convert.ToInt32(Session[Sys.userId]);


            using (entities = new wwbEntities())
            {
                userGroups = entities.UserGroups
                                    .Where(g => (g.user_id == userId))
                                    .ToList();

                string s = "";
                if (userGroups != null)
                {
                    foreach (UserGroups ug in userGroups)
                    {
                        group = entities.Groups
                                    .Where(g => g.group_id == ug.group_id)
                                    .FirstOrDefault();

                        userGroupList.Add(new Group(group.group_id, group.group_name));
                        s += group.group_name + " ";
                    }
                    ViewData[Sys.userGroups] = userGroupList;
                }

            }

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
                    /* Already exists */ //TODO : There is possibly a problem. check it 
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
        public JsonResult AddWord(int groupId, string word, string wordTr)
        {
            Words words;
            UserWords userWord;
            int userId = Convert.ToInt32(Session[Sys.userId]);
            try
            {
                using (entities = new wwbEntities())
                {
                    /* If group not exist then insert then just take id below */
                    words = entities.Words
                           .Where(w => w.word == word && w.word_translate == wordTr)
                           .FirstOrDefault();

                    if (words == null)
                    {
                        words = new Words();
                        words.group_id = groupId;
                        words.word = word;
                        words.word_translate = wordTr;

                        entities.Words.Add(words);
                        entities.SaveChanges();
                    }
                }

                using (entities = new wwbEntities())
                {
                    /* Already exists */
                    userWord = entities.UserWords
                                .Where(uw => (uw.user_id == userId && uw.word_id == words.word_id))
                                .FirstOrDefault();

                    if (userWord != null)
                    {
                        return Json(WordAddingStatus.AlreadyExist, JsonRequestBehavior.AllowGet);
                    }

                    userWord = new UserWords();
                    userWord.word_id = words.word_id;
                    userWord.user_id = userId;

                    entities.UserWords.Add(userWord);
                    entities.SaveChanges();
                }

            }
            catch (Exception e)
            {
                return Json(WordAddingStatus.Fail, JsonRequestBehavior.AllowGet);
            }


            return Json(WordAddingStatus.Success, JsonRequestBehavior.AllowGet);
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

            String line = "", responseJson = "";

            while ((line = reader.ReadLine()) != null)
                responseJson += line;



            return Json(responseJson, JsonRequestBehavior.AllowGet);
        }
    }
}