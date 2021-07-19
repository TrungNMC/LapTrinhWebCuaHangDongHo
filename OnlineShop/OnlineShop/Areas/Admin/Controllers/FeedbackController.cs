using Common;
using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class FeedbackController : BaseController
    {
        public ActionResult Index(string searchString, int page = 1, int pageSize = 5)
        {
            var dao = new FeedbackDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }
        public ActionResult Reply(int id)
        {
            var feedback = new FeedbackDao().ViewDetail(id);
            return View(feedback);
        }
        [HttpPost]
        public ActionResult Reply(Feedback feedback, string name, string reply, string email)
        {
            if (ModelState.IsValid)
            {
                feedback.Name = name;
                feedback.Reply = reply;
                feedback.Email = email;
                OnlineShopDbContext DB = new OnlineShopDbContext();
               var a= DB.Feedbacks.Find(feedback.ID);
                a.Status = true;
                DB.SaveChanges();
                var rep = new FeedbackDao().Reply(feedback);
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/client/template/reply.html"));
                content = content.Replace("{{CustomerName}}", name);               
                content = content.Replace("{{Reply}}", reply);
                new MailHelper().SendEmail(email, "Phản hồi từ KenStore", content);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new FeedbackDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }
    }
}