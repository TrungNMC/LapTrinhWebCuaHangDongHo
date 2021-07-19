
using BotDetect.Web.Mvc;
using Model.Dao;
using Model.EF;
using OnlineShop.Common;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
namespace OnlineShop.Controllers
{
    public class UserController : Controller
    {
        public const string USER_SESSION = "USER_SESSION";
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        // GET: User
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
    

        [HttpGet]
        public ActionResult Logout()
        {
            Session[CommonConstants.USER_SESSION] = null;
            return Redirect("/");
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                var result = dao.LoginForCus(model.UserName, Encryptor.GetMD5(model.Password));
                if (result == 1)
                {
                    var user = dao.GetById(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;
                    userSession.Name = user.Name;
                    userSession.Address = user.Address;
                    userSession.Email = user.Email;
                    userSession.Phone = user.Phone;
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    return Redirect("/");
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
                else
                {
                    ModelState.AddModelError("", "Đăng nhập không đúng!");
                }
            }
            return View("Login");
        }
        [HttpPost]
        [CaptchaValidation("CaptchaCode","registerCapcha","Mã xác nhận không đúng")]
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
        public JsonResult LoadProvince()
        {
            var xmlDoc = XDocument.Load(Server.MapPath(@"~/Assets/client/data/Provinces_Data.xml"));

            var xElements = xmlDoc.Element("Root").Elements("Item").Where(x => x.Attribute("type").Value == "province");
            var list = new List<ProvinceModel>();
            ProvinceModel province = null;
            foreach (var item in xElements)
            {
                province = new ProvinceModel();
                province.ID = int.Parse(item.Attribute("id").Value);
                province.Name = item.Attribute("value").Value;
                list.Add(province);

            }
            return Json(new
            {
                data = list,
                status = true
            });
        }
        public JsonResult LoadDistrict(int provinceID)
        {
            var xmlDoc = XDocument.Load(Server.MapPath(@"~/Assets/client/data/Provinces_Data.xml"));

            var xElement = xmlDoc.Element("Root").Elements("Item")
                .Single(x => x.Attribute("type").Value == "province" && int.Parse(x.Attribute("id").Value) == provinceID);

            var list = new List<DistrictModel>();
            DistrictModel district = null;
            foreach (var item in xElement.Elements("Item").Where(x => x.Attribute("type").Value == "district"))
            {
                district = new DistrictModel();
                district.ID = int.Parse(item.Attribute("id").Value);
                district.Name = item.Attribute("value").Value;
                district.ProvinceID = int.Parse(xElement.Attribute("id").Value);
                list.Add(district);

            }
            return Json(new
            {
                data = list,
                status = true
            });
        }

    }
}