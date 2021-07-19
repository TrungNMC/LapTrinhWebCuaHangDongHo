using Common;
using Model.Dao;
using Model.EF;
using MoMo;
using Newtonsoft.Json.Linq;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private const string CartSession = "CartSession";
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }
        public JsonResult Delete(long id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.Product.ID == id);
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public JsonResult Update(String cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];
            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

        public ActionResult AddItem(long productId, int quantity)
        {
            var product = new ProductDao().ViewDetail(productId);
            UserLogin userLogin = (UserLogin)Session[OnlineShop.Common.CommonConstants.USER_SESSION];
            //if (userLogin == null)
            //{

            //    var item = new CartItem();
            //    item.Product = product;
            //    item.Quantity = quantity;
            //    var list = new List<CartItem>();
            //    list.Add(item);
            //    //gán vào session
            //    Session[CartSession] = list;
            //    return RedirectToAction("Index");
            //}    
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.Product.ID == productId))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ID == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                Session[CartSession] = list;
            }
            else
            {

                //tạo mới cart item
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                //gán vào session
                Session[CartSession] = list;
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Payment()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }
        public string momo(string cost)
        {
            string endpoint = "https://payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMODDI520210624";
            string accessKey = "xryFOk958utQJR3T";
            string serectkey = "art4TPJhFphYnpVLIDX9pIWKcXybGJw3";
            string orderInfo = "Thanh Toán Đơn Hàng";
            //string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            //string partnerCode = "MOMOZ9MI20210624";
            //string accessKey = "0b0X3Io8pP8F21Y2";
            //string serectkey = "SWjXF22dGhOxZU3waGTdeSECfX6LmhaA";
            //string orderInfo = "Đơn Hàng Mới";
            string returnUrl = "https://localhost:44393/thanh-toan";
            string notifyurl = "https://localhost:44393/hoan-thanh";

            string amount = cost ;
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            //   log.Debug("rawHash = " + rawHash);

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);
            // log.Debug("Signature = " + signature);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);


            //yes...
            return jmessage.GetValue("payUrl").ToString();


        

        }

        public JsonResult thanhtoan(string shipName, string mobile,string address, string email ,bool kiemTra)
        {

            var order = new Order();
            order.CreatedDate = DateTime.Now;
            order.ShipAddress = address;
            order.ShipMobile = mobile;
            order.ShipName = shipName;
            order.ShipEmail = email;
            UserLogin userLogin = new UserLogin();
            userLogin.Email = email;
            userLogin.Name = shipName;
            userLogin.Phone = mobile;
            userLogin.Address = address;
           Session[OnlineShop.Common.CommonConstants.USER_SESSION] = userLogin;
            try
            {
                var cart = (List<CartItem>)Session[CartSession];
                var detailDao = new OrderDetailDao();
                decimal total = 0;
                long productId = 0;
                int cost = 0;
             foreach (var item in cart)
                    {
                        var orderDetail = new OrderDetail();
                        orderDetail.ProductID = item.Product.ID;
                        orderDetail.Price = item.Product.Price;
                        orderDetail.Quantity = item.Quantity;
                        total += (item.Product.Price.GetValueOrDefault(0) * item.Quantity);
                        cost += int.Parse((item.Product.Price.GetValueOrDefault(0) * item.Quantity).ToString());
                        productId = orderDetail.ProductID;

                    }
                Session["odert"] = order;
                if (kiemTra == true)
                {
                                  
                    return Json(new { url=momo(cost.ToString()) }, JsonRequestBehavior.AllowGet);
                }
            
                }
            catch (Exception ex)
            {
                return Json (new { url= "/loi-thanh-toan" },JsonRequestBehavior.AllowGet);
            }
            return Json(new { url = "/hoan-thanh" }, JsonRequestBehavior.AllowGet);
        }
       
        public string guimail(string Lydo, long ID)
        {
            var dao = new OrderDao();
            var order = new OrderDao().ViewDetail(ID);
            order.Status = 5;
            var result = dao.Update(order);
            // var rep = new FeedbackDao().Reply(feedback);
            string content = "Mã đơn hàng  : "+order.ID+" Của khách hàng "+ order.ShipName +" đã hủy lí do " +Lydo;
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            new MailHelper().SendEmail(toEmail, "Đơn hàng đã bị hủy", content);

            return "";

        }
        public ActionResult Success()
        {
            var cart = (List<CartItem>)Session[CartSession];
            var detailDao = new OrderDetailDao();
            decimal total = 0;
            long productId = 0;
            var order = (Order) Session["odert"];
            var id = new OrderDao().Insert(order);
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/client/template/neworder.html"));
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail();
                orderDetail.ProductID = item.Product.ID;
                orderDetail.OrderID = id;
                orderDetail.Price = item.Product.Price;
                orderDetail.Quantity = item.Quantity;
               detailDao.Insert(orderDetail);
                productId = orderDetail.ProductID;
                total += (item.Product.Price.GetValueOrDefault(0) * item.Quantity);
                content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/client/template/neworder.html"));
                content = content.Replace("{{CustomerName}}", order.ShipName);
                // m lam  ko ddc ha
                content = content.Replace("{{ProductId}}", item.Product.Name);
                content = content.Replace("{{Phone}}", order.ShipMobile);
                content = content.Replace("{{Email}}", order.ShipEmail);
                content = content.Replace("{{Address}}", order.ShipAddress);
                content = content.Replace("{{Total}}", total.ToString("N0"));
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                //  string toEmail = "quangtruong112000.93@gmail.com";
                new MailHelper().SendEmail(order.ShipEmail, "Đơn hàng mới từ OnlineShop", content);
                new MailHelper().SendEmail(toEmail, "Đơn hàng mới từ OnlineShop", content);

            }
        
            Session[CartSession]=null;
            Session["odert"]=null;
            return View();
        }
    }
}