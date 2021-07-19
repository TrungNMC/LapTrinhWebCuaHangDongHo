using Common;
using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            var model = new ContactDao().GetActiveContact();
            return View(model);
        }
        [HttpPost]
        public JsonResult Send(string name, string mobile, string address, string email, string content)
        {
            var feedback = new Feedback();
            feedback.Name = name;
            feedback.Email = email;
            feedback.CreatedDate = DateTime.Now;
            feedback.Phone = mobile;
            feedback.Content = content;
            feedback.Address = address;
            var id = new ContactDao().InsertFeedback(feedback);
            string noidung = System.IO.File.ReadAllText(Server.MapPath("~/Assets/client/template/contact.html"));
            noidung = noidung.Replace("{{CustomerName}}", name);
            noidung = noidung.Replace("{{Phone}}", mobile);
            noidung = noidung.Replace("{{Email}}", email);
            noidung = noidung.Replace("{{Address}}", address);
            noidung = noidung.Replace("{{Content}}", content);
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            //new MailHelper().SendMail(email, "Đơn hàng mới từ OnlineShop", noidung);
            new MailHelper().SendEmail(toEmail, "Phản hồi từ khách hàng:", noidung);
            if (id > 0)
            {
                return Json(new
                {
                    status = true
                });     
                
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }
    }
}