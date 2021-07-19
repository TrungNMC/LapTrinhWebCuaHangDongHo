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
    public class ProductCategoryController : BaseController
    {
        public ActionResult Index(string searchString, int page = 1, int pageSize = 5)
        {
            var dao = new ProductCategoryDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);

            ViewBag.SearchString = searchString;

            return View(model);
        }

       // [HttpGet]
        //public ActionResult Create()
        //{
        //    SetViewBag();
        //    OnlineShopDbContext db = new OnlineShopDbContext();
            

        //  //  ViewBag.lstLop = new SelectList(ltsLop, "Ma_Lop", "TenLop");
        //    return View();
        //}

        
        public JsonResult Create(string ten,string title)
        {
          
                var dao = new ProductCategoryDao();
            ProductCategory productcate = new ProductCategory();
            productcate.MetaTitle = title;
            productcate.Name = ten;
                long id = dao.Insert(productcate);
                if (id > 0)
                {
                return Json(new
                {
                    code = 100
                }, JsonRequestBehavior.AllowGet) ; 
                }
                else
                {
                return Json(new
                {
                    code = 200
                }, JsonRequestBehavior.AllowGet);

            }
         

        }
        public JsonResult Edit( long ma, string ten, string title)
        {

            var dao = new ProductCategoryDao();
            ProductCategory productcate = new ProductCategory();
            productcate.MetaTitle = title;
            productcate.ID =  ma;
            productcate.Name = ten;
            var result = dao.Update(productcate);
            if (result)
            {

                return Json(new
                {
                    code = 100
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    code = 200
                }, JsonRequestBehavior.AllowGet);

            }


        }
      
        public ActionResult Delete(int id)
        {
            new ProductCategoryDao().Delete(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new ProductDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }
        public void SetViewBag(long? selectedId = null)
        {
            var dao = new ProductCategoryDao();
            ViewBag.CategoryID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
        }
    }
}