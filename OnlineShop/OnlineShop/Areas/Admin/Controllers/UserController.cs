using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.EF;
using Model.Dao;
using OnlineShop.Common;
using PagedList;

namespace OnlineShop.Areas.Admin.Controllers
{

    public class UserController : BaseController
    {
        // GET: Admin/User
        public ActionResult Index(string searchString, int page = 1, int pageSize = 5)
        {
            var dao = new AdminDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Ad admin)
        {
            if (ModelState.IsValid)
            {
                var dao = new AdminDao();
                if (dao.ckeckUserName(admin.UserName))
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
                    return View("Create");
                }
                else if (dao.checkEmail(admin.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                    return View("Create");
                }
                else
                {
                    var encryptedMd5Pas = Encryptor.GetMD5(admin.Password);
                    admin.Password = encryptedMd5Pas;
                    long id = dao.Insert(admin);
                    if (id > 0)
                    {
                        SetAlert("Thêm thành công", "success");
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm không thành công");
                    }
                }             
            }
            SetViewBag();
            return View("Create");
        }
        public ActionResult Edit(int id)
        {
            var admin = new AdminDao().ViewDetail(id);
            SetViewBag();
            return View(admin);
        }
        [HttpPost]
        public ActionResult Edit(Ad admin)
        {
            if (ModelState.IsValid)
            {
                var dao = new AdminDao();
                if (!string.IsNullOrEmpty(admin.Password))
                {
                    var encryptedMd5Pas = Encryptor.GetMD5(admin.Password);
                    admin.Password = encryptedMd5Pas;
                }
                var result = dao.Update(admin);
                if (result)
                {
                    SetAlert("Sửa thành công", "success");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công");
                }
            }
            SetViewBag();
            return View("Index");
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            new AdminDao().Delete(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new AdminDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }
        public ActionResult Logout()
        {
            Session[CommonConstants.ADMIN_SESSION] = null;
            return Redirect("/admin/login/");
        }
        public void SetViewBag()
        {
            var dao = new AdminDao();
            ViewBag.GroupID = new SelectList(dao.ListAll(), "ID", "Name");
        }
    }
}