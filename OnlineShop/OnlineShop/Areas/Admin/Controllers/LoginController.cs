using Model.Dao;
using Model.EF;
using OnlineShop.Areas.Admin.Models;
using OnlineShop.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var dao = new AdminDao();
                var result = dao.Login(model.UserName, Encryptor.GetMD5(model.Password));
                if (result == 1)
                {
                    var admin = dao.GetById(model.UserName);
                    var adminSesstion = new UserLogin();
                    adminSesstion.UserName = admin.UserName;
                    adminSesstion.UserID = admin.ID;
                    adminSesstion.GroupID = admin.GroupID;
                    adminSesstion.Name = admin.Name;
                    var listCredentials = dao.GetListCredential(model.UserName);
                    Session.Add(CommonConstants.ADMIN_SESSION, listCredentials);
                    Session.Add(CommonConstants.ADMIN_SESSION, adminSesstion);
                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản đang bị khóa.");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else if (result == -3)
                {
                    ModelState.AddModelError("", "Tài khoản của bạn không có quyền đăng nhập.");
                }
                else
                {
                    ModelState.AddModelError("", "Đăng nhập không đúng!");
                }
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            Ad admin = (Ad)Session["adchangepass"];
            var ad = new AdminDao().ViewDetail(admin.ID);
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(Ad admin)
        {
            Ad admin1 = (Ad)Session["adchangepass"];
            admin.ID = admin1.ID;
            var dao = new AdminDao();
            if (!string.IsNullOrEmpty(admin.Password))
            {
                var encryptedMd5Pas = Encryptor.GetMD5(admin.Password);
                admin.Password = encryptedMd5Pas;
            }
            var result = dao.ForgotPass(admin);
            return View("Index");
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            LoginModel login = new LoginModel();
            login.Password = "1111";
            login.UserName = "1111";
            ViewBag.Message = "";
            return View(login);
        }
        [HttpPost]
        public ActionResult ForgotPassword(LoginModel model)
        {
            var dao = new AdminDao();
            if (model.Email != null)
            {
                if (new OnlineShopDbContext().Admins.SingleOrDefault(x => x.Email.Equals(model.Email)) != null)
                {
                    Session["adchangepass"] = new OnlineShopDbContext().Admins.SingleOrDefault(x => x.Email.Equals(model.Email));
                    new MailHelper().SendMail(model.Email, "Xác nhận mật khẩu", "Nhấn vào dường dẫn để đổi mật khẩu <a href = https://localhost:44311/Admin/Login/ChangePassword >Xác Nhận</a> ");
                    ViewBag.Message = "Vui lòng kiểm tra mail của bạn";
                }
                else
                {
                    ModelState.AddModelError("", "Mail bạn nhập không tồn tại!!");
                    ViewBag.Message = "";
                }
            }
            else
            {
                ModelState.AddModelError("", "Vui lòng nhập email!!");
                ViewBag.Message = "";
            }

            return View();
        }

        public class MailHelper
        {
            public void SendMail(string toEmailAddress, string subject, string content)
            {
                var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                var fromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName1"].ToString();
                var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                var smtpPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();

                bool enabledSsl = bool.Parse(ConfigurationManager.AppSettings["EnabledSSL"].ToString());

                string body = content;
                MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, fromEmailDisplayName), new MailAddress(toEmailAddress));
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                var client = new SmtpClient();
                client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
                client.Host = smtpHost;
                client.EnableSsl = enabledSsl;
                client.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
                client.Send(message);
            }
        }
    }
}