using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            ViewBag.countbenefit = new StatisticalDao().countbenefit();
            ViewBag.countrevenue = new StatisticalDao().countrevenue();
            ViewBag.counttask = new OrderDao().counttask();
            ViewBag.countfeedback = new FeedbackDao().countfeedback();
            ViewBag.listSta = new StatisticalDao().listStatistical();
          //  ViewBag.revenue5 = new StatisticalDao().revenue5();
         //   ViewBag.revenue6 = new StatisticalDao().revenue6();
          //  ViewBag.benefit5 = new StatisticalDao().benefit5();
            ViewBag.benefit6 = new StatisticalDao().benefit6();
          //  ViewBag.revenue4 = new StatisticalDao().revenue4();
           // ViewBag.benefit4 = new StatisticalDao().benefit4();
            return View();
        }
       
        public JsonResult thongke1()
        {
            OnlineShopDbContext db = new OnlineShopDbContext();
            // List<OnlineShop.Areas.Admin.Models.ThongKeDanhMuc> thongKeDanhMucs = new List<Models.ThongKeDanhMuc>();
            DateTime date = DateTime.Now.AddDays(-6);
            string[] arr = new string[7];
            double[] arr1 = new double[7];
            for (int i = 1; i <8; i++)
            {
                var order = db.Orders.Where(x=>x.Status==4 && x.CreatedDate.Value.Day==date.Day
                && x.CreatedDate.Value.Month==date.Month && x.CreatedDate.Value.Year == date.Year);
                arr1[i-1] = 0;
                arr[i-1] = date.Day + "/" + date.Month;
                foreach (var item in order.ToList())
                {
                    var tongtien = db.OrderDetails.Where(x => x.OrderID == item.ID).ToList().Sum(x => x.Price);
                    arr1[i-1] += double.Parse(tongtien.ToString());
                }

                date = date.AddDays(1);
            }
            return Json(new
            {
                arr,
                arr1
            }) ;



        }
            public JsonResult thongke()
        {
            OnlineShopDbContext db = new OnlineShopDbContext();
            List<OnlineShop.Areas.Admin.Models.ThongKeDanhMuc> thongKeDanhMucs = new List<Models.ThongKeDanhMuc>();
         
              
            var list = db.Orders.ToList();
            foreach (var item1 in list.Where(x=>x.Status==4))
            {
                foreach (var item in db.OrderDetails.Select(x => x).ToList())
                {
                    var sp = db.Products.Find(item.ProductID);
                    if (!thongKeDanhMucs.Exists(x => x.id == sp.CategoryID))
                    {
                        try
                        {
                            thongKeDanhMucs.Add(new Models.ThongKeDanhMuc
                            {
                                id = (long)sp.CategoryID,
                                name = db.ProductCategories.Find(sp.CategoryID).Name,
                                sl = (int)item.Quantity


                            }

);
                        }
                        catch { }
                        
                    }
                    else
                    {

                        thongKeDanhMucs.SingleOrDefault(x => x.id == sp.CategoryID).sl += (int)item.Quantity;

                    }
                }
            }
         
            return Json(new
            {
                arr = thongKeDanhMucs
            });

        }
    }
}