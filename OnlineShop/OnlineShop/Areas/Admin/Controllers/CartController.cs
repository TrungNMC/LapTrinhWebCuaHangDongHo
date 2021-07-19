using Common;
using Model.Dao;
using Model.EF;
using Model.ViewModel;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class CartController : BaseController
    {     
        public ActionResult Index()
        {
            var dao = new OrderDao();
            ViewBag.listOrder = dao.ListOrder();           
            return View();
        }              
    }
}