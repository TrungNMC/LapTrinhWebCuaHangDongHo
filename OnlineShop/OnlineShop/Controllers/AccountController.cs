using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BotDetect.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Model.Dao;
using Model.EF;
using OnlineShop.Common;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {


        }
        //[AllowAnonymous]
        //public ActionResult DoiMatKhau()
        //{
        //    try
        //    {
        //        var tk = (TaiKhoan)Session["user"];

        //        RegisterViewModel RegisterViewMode = new RegisterViewModel();
        //        RegisterViewMode.Email = tk.MaTK;
        //        RegisterViewMode.Name = tk.Ten;
        //        return View(RegisterViewMode);
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //}
        // POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DoiMatKhau(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        TracNghiemDB tracNghiemDB = new TracNghiemDB();
        //        TaiKhoan taiKhoan1 = tracNghiemDB.TaiKhoans.Find(model.Email);
        //        taiKhoan1.MaTK = model.Email;
        //        string mk = GetMD5(model.ConfirmPassword);
        //        taiKhoan1.MatKhau = mk;
        //        taiKhoan1.Quyen = false;
        //        taiKhoan1.NgayTao = DateTime.UtcNow;
        //        taiKhoan1.Ten = model.Name;
        //        taiKhoan1.TrangThai = true;
        //        tracNghiemDB.SaveChanges();
        //        Session["user"] = taiKhoan1;
        //        return RedirectToAction("Index", "Home");

        //        // ModelState.AddModelError("", "Email Đã Tồn Tại");
        //    }
        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            //uthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            // AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);


            ViewBag.ReturnUrl = returnUrl;
            return PartialView();
        }



        //
        // POST: /Account/Login

        [AllowAnonymous]
        public JsonResult KiemTra(string email, string pass)
        {

            var dao = new UserDao();
            var result = dao.LoginForCus(email, GetMD5(pass));
            if (result == 1)
            {
                var user = dao.GetById(email);
                var userSession = new UserLogin();
                userSession.UserName = user.UserName;
                userSession.UserID = user.ID;
                userSession.Name = user.Name;
                userSession.Address = user.Address;
                userSession.Email = user.Email;
                userSession.Phone = user.Phone;
                Session.Add(CommonConstants.USER_SESSION, userSession);
                var cart = Session["CartSession"];
                var list = new List<CartItem>();
                if (cart != null)
                {
                    return Json(new { code = 600 }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 100 }, JsonRequestBehavior.AllowGet);
            }
            else if (result == 0)
            {
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);

            }
            else if (result == -1)
            {
                return Json(new { code = 300 }, JsonRequestBehavior.AllowGet);

            }
            else if (result == -2)
            {
                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);

            }


        }
        //
        // GET: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DoiMatKhau1(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        TracNghiemDB tracNghiemDB = new TracNghiemDB();
        //        TaiKhoan taiKhoan1 = tracNghiemDB.TaiKhoans.Find(model.Email);
        //        taiKhoan1.MaTK = model.Email;
        //        string mk = GetMD5(model.ConfirmPassword);
        //        taiKhoan1.MatKhau = mk;
        //        taiKhoan1.Quyen = false;
        //        taiKhoan1.NgayTao = DateTime.UtcNow;
        //        taiKhoan1.Ten = model.Name;
        //        taiKhoan1.TrangThai = true;
        //        tracNghiemDB.SaveChanges();
        //        Session["user"] = taiKhoan1;
        //        return RedirectToAction("Index", "Home");

        //        // ModelState.AddModelError("", "Email Đã Tồn Tại");
        //    }
        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCode", "registerCapcha", "Mã xác nhận không đúng")]
        public ActionResult Register(RegisterModel model)
        {
            var dao = new UserDao();
            if (dao.ckeckUserName(model.UserName))
            {
                ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
            }
            else if (dao.checkEmail(model.Email))
            {
                ModelState.AddModelError("", "Email đã tồn tại");
            }
            else if (model.Password == model.ConfirmPassword)
            {
                var user = new User();
                user.UserName = model.UserName;
                user.Name = model.Name;
                user.Password = Encryptor.GetMD5(model.Password);
                user.Phone = model.Phone;
                user.Email = model.Email;
                user.Address = model.Address;
                user.CreatedDate = DateTime.Now;
                user.DistrictID = int.Parse(model.DistrictID);
                user.ProvinceID = int.Parse(model.ProvinceID);
                user.Status = true;
                if (!string.IsNullOrEmpty(model.DistrictID))
                {
                    user.DistrictID = int.Parse(model.DistrictID);
                }
                if (!string.IsNullOrEmpty(model.ProvinceID))
                {
                    user.ProvinceID = int.Parse(model.ProvinceID);
                }
                var result = dao.Insert(user);
                if (result > 0)
                {
                    ViewBag.Success = "Đăng kí thành công";
                    model = new RegisterModel();
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Đăng kí không thành công");
                }
            }
            return View("Register");
        }
        //[AllowAnonymous]
        //public ActionResult CreateAccount()
        //{
        //    try
        //    {
        //        var tk = (TaiKhoan)Session["taotk"];

        //        if (tk != null)
        //        {

        //            TracNghiemDB tracNghiemDB = new TracNghiemDB();
        //            tracNghiemDB.TaiKhoans.Add(tk);
        //            tracNghiemDB.SaveChanges();
        //            Session.Add("user", tk);
        //            return RedirectToAction("Index", "Home");
        //        }

        //    }
        //    catch { }

        //    return View("Error");


        //}
        public string GetMD5(string chuoi)
        {
            string str_md5 = "";
            byte[] mang = System.Text.Encoding.UTF8.GetBytes(chuoi);

            MD5CryptoServiceProvider my_md5 = new MD5CryptoServiceProvider();
            mang = my_md5.ComputeHash(mang);

            foreach (byte b in mang)
            {
                str_md5 += b.ToString("X2");
            }

            return str_md5;
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //[AllowAnonymous]
        //public ActionResult DoiMatKhau1()
        //{
        //    var tk = (TaiKhoan)Session["user"];
        //    RegisterViewModel RegisterViewMode = new RegisterViewModel();
        //    RegisterViewMode.Email = tk.MaTK;
        //    RegisterViewMode.Name = tk.Ten;
        //    return View(RegisterViewMode);
        //}

        //
        // POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel loginInfo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var Tk = new TracNghiemDB().TaiKhoans.SingleOrDefault(x => x.MaTK.Equals(loginInfo.Email) && x.MatKhau != null);


        //        if (Tk != null)
        //        {
        //            if (Tk.TrangThai == true)
        //            {
        //                Session["user"] = Tk;

        //                SendEmail(loginInfo.Email, "Xác nhận mật khẩu", "Please reset your password by clicking <a class='btn btn-success' href =https://localhost:44343/Account/DoiMatKhau1 >Xác Nhận</a> ");
        //                return View("ForgotPasswordConfirmation");
        //            }

        //            ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa");

        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Email bạn nhập không tồn tại");
        //        }

        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(loginInfo);
        //}

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }




        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return RedirectToAction("Index", "Home");
            }
            OnlineShopDbContext db = new OnlineShopDbContext();
            var Tk = db.Users.SingleOrDefault(x => x.Email.Equals(loginInfo.Email));
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            if (Tk == null)
            {
                User taiKhoan1 = new User();
                taiKhoan1.Email = loginInfo.Email;
                taiKhoan1.Name = loginInfo.DefaultUserName;
                taiKhoan1.UserName = loginInfo.Email;
                taiKhoan1.Address = null;
                taiKhoan1.Status = true;
                taiKhoan1.Password = GetMD5("123");
                //  TracNghiemDB tracNghiemDB = new TracNghiemDB();
                Tk = taiKhoan1;

                db.Users.Add(taiKhoan1);
                db.SaveChanges();
                Tk.ID = db.Users.SingleOrDefault(x => x.Email.Equals(loginInfo.Email)).ID;
            }
            var userSession = new UserLogin();
            userSession.UserName = Tk.UserName;
            userSession.UserID = Tk.ID;
            userSession.Name = Tk.Name;
            userSession.Email = Tk.Email;

            Session.Add(CommonConstants.USER_SESSION, userSession);

            return RedirectToAction("Index", "Home");

        }
        public void SendEmail(string address, string subject, string message)
        {
            string email = "tmooquiz40@gmail.com";
            string password = "0353573467";

            var loginInfo = new NetworkCredential(email, password);
            var msg = new System.Net.Mail.MailMessage();
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);

            msg.From = new MailAddress(email);
            msg.To.Add(new MailAddress(address));
            msg.Subject = subject;
            msg.Body = message;
            msg.IsBodyHtml = true;

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(msg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}