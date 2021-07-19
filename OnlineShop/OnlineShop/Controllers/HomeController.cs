    using Model.Dao;
using MoMo;
using Newtonsoft.Json.Linq;
using OnlineShop.Common;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Model.EF;
namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {      
        // GET: Home
      
        public ActionResult Index()
        {
            ViewBag.Slides = new SlideDao().ListAll();
            var productDao = new ProductDao();
            ViewBag.ListNewProducts = productDao.ListNewProduct(4);
            ViewBag.ListFeatureProducts = productDao.ListFeatureProduct(4);
            ViewBag.Title = ConfigurationManager.AppSettings["HomeTitle"];
            ViewBag.Keywords = ConfigurationManager.AppSettings["HomeKeyword"];
            ViewBag.Descriptions = ConfigurationManager.AppSettings["HomeDescription"];
            return View();
        }
        public ActionResult Lichsu(long id)
        {
       var user=  (UserLogin)  Session[OnlineShop.Common.CommonConstants.USER_SESSION];


            return View(new OnlineShopDbContext().Orders.Where(x=>x.Status==id && user.UserName.Equals(x.ShipEmail)).ToList());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 3600*24)]
        public ActionResult MainMenu()
        {
            var model = new MenuDao().ListByGroupId(1);
            return PartialView(model);
        }
           public string guimail(string Lydo, string Name, string Email, long ID) {
            var dao = new OrderDao();
            var order = new OrderDao().ViewDetail(ID);
            order.Status = 5;
            var result = dao.Update(order);
            // var rep = new FeedbackDao().Reply(feedback);
            string content = "Xin chào " + Name + " Shop mình xin lỗi quý khách đơn hàng của quý khách đã bị hủy vì" + Lydo + " xin quý khách thồnh cảm vì sự cố của bên tôi.........";
          
            
            return "";
        }
        public ActionResult TopMenu()
        {
            var model = new MenuDao().ListByGroupId(2);
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult HeaderCart()
        {
            var cart = Session[CommonConstants.CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return PartialView(list);
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public ActionResult Footer()
        {
            var model = new FooterDao().GetFooter();
            return PartialView(model);
        }
    }
}