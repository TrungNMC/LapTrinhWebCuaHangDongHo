using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Dao;
using Model.EF;
using OnlineShop.Common;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        public const string USER_SESSION = "USER_SESSION";
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public PartialViewResult ProductCategory()
        {
            var model = new ProductCategoryDao().ListAll();
            return PartialView(model);
        }
        public JsonResult ListName(string q)
        {
            var data = new ProductDao().ListName(q);
            return Json(new
            {
                data = data,
                status = true

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Category(long cateId, int page = 1, int pageSize = 8)
        {
            var category = new CategoryDao().ViewDetail(cateId);
            ViewBag.Category = category;
            int totalRecod = 0;
            var model = new ProductDao().ListByCategoryId(cateId, ref totalRecod, page, pageSize);
            ViewBag.Total = totalRecod;
            ViewBag.Page = page;
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling((double)(totalRecod / pageSize));
            ViewBag.totalPage = totalPage;
            ViewBag.maxPage = maxPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            ViewBag.Last = maxPage;
            ViewBag.First = 1;
            ViewBag.Star = new RatingDao().Star();
            return View(model);
        }
        public ActionResult Search(string keyword, int page = 1, int pageSize = 10)
        {
            int totalRecord = 0;
            var model = new ProductDao().Search(keyword, ref totalRecord, page, pageSize);
            ViewBag.Total = totalRecord;
            ViewBag.Page = page;
            ViewBag.Keyword = keyword;
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling((double)(totalRecord / pageSize));
            ViewBag.totalPage = totalPage;
            ViewBag.maxPage = maxPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            ViewBag.Last = maxPage;
            ViewBag.First = 1;
            ViewBag.Star = new RatingDao().Star();
            return View(model);
        }
        
        public ActionResult Detail(long id, int page = 1, int pageSize = 3)
        {
            int totalRecord = 0;
            var product = new ProductDao().ViewDetail(id);
            ViewBag.Category = new ProductCategoryDao().ViewDetail(product.CategoryID.Value);
            ViewBag.RelatedProducts = new ProductDao().ListRelatedProduct(id);
            ViewBag.ShowComment = new RatingDao().ShowComment(id, ref totalRecord, page, pageSize);
            ViewBag.SumStar = new RatingDao().SumStar(id);
            ViewBag.Sum5Star = new RatingDao().Sum5Star(id);
            ViewBag.Sum4Star = new RatingDao().Sum4Star(id);
            ViewBag.Sum3Star = new RatingDao().Sum3Star(id);
            ViewBag.Sum2Star = new RatingDao().Sum2Star(id);
            ViewBag.Sum1Star = new RatingDao().Sum1Star(id);
            ViewBag.Percent5Star = new RatingDao().Percent5Star(id);
            ViewBag.Percent4Star = new RatingDao().Percent4Star(id);
            ViewBag.Percent3Star = new RatingDao().Percent3Star(id);
            ViewBag.Percent2Star = new RatingDao().Percent2Star(id);
            ViewBag.Percent1Star = new RatingDao().Percent1Star(id);
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling((double)(totalRecord / pageSize));
            ViewBag.totalPage = totalPage;
            ViewBag.maxPage = maxPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            ViewBag.Last = maxPage;
            ViewBag.First = 1;
            ViewBag.Star = new RatingDao().Star();
            Session["ID"] = id;
            return View(product);
        }
        public JsonResult Rating(string comment, int sao)
        {
            long id = (long)Session["ID"];
            var product = new ProductDao().ViewDetail(id);
            ViewBag.Star = new RatingDao().Star();
            var usersession = (UserLogin)Session[OnlineShop.Common.CommonConstants.USER_SESSION];
            Rating rating = new Rating();
            rating.Comment = comment;
            rating.Rate = sao;
            long ratingid = new RatingDao().Insert(rating, id, usersession.UserID);
            return Json(new
            {
                MetaTitle = product.MetaTitle,
                id = product.ID,
            });            
        }
        public ActionResult Store(int page = 1, int pageSize = 9)
        {
            int totalRecord = 0;
            ViewBag.listpromotion = new ProductDao().ListPromotion();
            ViewBag.category = new ProductCategoryDao().ListAll();
            var product = new ProductDao().ProductPaging(ref totalRecord, page, pageSize);
            ViewBag.Total = totalRecord;
            ViewBag.Page = page;
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling((double)(totalRecord / pageSize));
            ViewBag.totalPage = totalPage;
            ViewBag.maxPage = maxPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            ViewBag.Last = maxPage;
            ViewBag.First = 1;
            ViewBag.Star = new RatingDao().Star();
            return View(product);
        }
        [HttpPost]
        public ActionResult SearchPrice(int page = 1, int pageSize = 9)
        {
            string pricemin = (Request.Form["pricemin"].ToString());
            string pricemax = (Request.Form["pricemax"].ToString());
            int totalRecord = 0;
            ViewBag.listpromotion = new ProductDao().ListPromotion();
            ViewBag.category = new ProductCategoryDao().ListAll();
            ViewBag.SearchPrice = new ProductDao().SearchPrice(Convert.ToInt64(pricemin), Convert.ToInt64(pricemax), ref totalRecord, page, pageSize);
            ViewBag.Total = totalRecord;
            ViewBag.Page = page;
            ViewBag.Pricemin = pricemin;
            ViewBag.Pricemax = pricemax;
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling((double)(totalRecord / pageSize));
            ViewBag.totalPage = totalPage;
            ViewBag.maxPage = maxPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            ViewBag.Last = maxPage;
            ViewBag.First = 1;
            ViewBag.Star = new RatingDao().Star();
            return View();
        }
    }
}