using Model.Dao;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.EF;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class StatisticalController : BaseController
    {
        // GET: Admin/Statistical
        class thongke
        {
            public string Thang;
            public int doanh;
            public int loi;
            public string benefit;
            public string revenue;

        }

        public ActionResult Index()
        {
            var dao = new StatisticalDao();
            ViewBag.listSta = dao.listStatistical();
          
            

            return View();
        }
        public JsonResult ThongkeTheoThang()
        {

            //    int arr =[];

            DateTime dateTime = DateTime.Now;
            Session["ngaykt="] = DateTime.Now;
            int n = dateTime.Month;
            Session["ngaybd="] = dateTime.AddDays(-n);
            thongke[] arr = new thongke[n];
            for (int i = 0; i < n; i++)
            {
                thongke thongke = new thongke();
                thongke.Thang = "Tháng " + (i + 1) + " - " + 2021;
                thongke.doanh = (int)new StatisticalDao().revenue4(i+1);
                thongke.loi = (int)new StatisticalDao().benefit4(i+1);
                thongke.revenue = new StatisticalDao().revenue4(i+1).ToString("N0");
                thongke.benefit = new StatisticalDao().benefit4(i+1).ToString("N0");
                arr[i] = thongke;
            }


            return Json(new
            {
                arr,

            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ThongkeTheonngay(string ngaybd, string ngayKT) {

            //    int arr =[];

            DateTime dateTime = DateTime.Parse(ngaybd);
            Session["ngaykt="] = DateTime.Parse(ngayKT);
            DateTime dateTime1 = DateTime.Parse(ngayKT);
          
            Session["ngaybd="] = dateTime;
         //   thongke[] arr = new thongke[n];
            OnlineShopDbContext db = new OnlineShopDbContext();
            // List<OnlineShop.Areas.Admin.Models.ThongKeDanhMuc> thongKeDanhMucs = new List<Models.ThongKeDanhMuc>();
        //    DateTime date = dateTime.AddDays(-n);
            List<thongke> arr = new List<thongke>();
           List<double> arr1 = new List<double>();
         
                var order = db.Orders.Where(x=>x.CreatedDate >=dateTime && x.CreatedDate<=dateTime1).OrderBy(x=>x.CreatedDate);

              //  arr[i - 1] = date.Day + "/" + date.Month+"/"+date.Year;
                foreach (var item in order.ToList())
                {
                try
                {
                    var tongtien = db.OrderDetails.Where(x => x.OrderID == item.ID).ToList().Sum(x => x.Price);
                    if (!arr.Exists(x => x.Thang.Contains(item.CreatedDate.Value.Day + "/" + item.CreatedDate.Value.Month + "/" + item.CreatedDate.Value.Year)))
                    {

                        arr1.Add(double.Parse(tongtien.ToString()));
                        thongke thongke = new thongke();
                        thongke.Thang = item.CreatedDate.Value.Day + "/" + item.CreatedDate.Value.Month + "/" + item.CreatedDate.Value.Year;
                        thongke.doanh= (int)new StatisticalDao().revenue5(item.CreatedDate);
                        thongke.loi = (int)new StatisticalDao().benefit5(item.CreatedDate);
                        thongke.revenue = new StatisticalDao().revenue5(item.CreatedDate).ToString("N0");
                        thongke.benefit = new StatisticalDao().benefit5(item.CreatedDate).ToString("N0");
                        arr.Add(thongke);

                    }
                    //else
                    //{
                    //    for (int i = 0; i < arr.Count; i++)
                    //    {
                    //        if (arr[i].Equals(item.CreatedDate.Value.Day + "/" + item.CreatedDate.Value.Month + "/" + item.CreatedDate.Value.Year))
                    //        {

                    //            var tongtien1 = db.OrderDetails.Where(x => x.OrderID == item.ID).ToList().Sum(x => x.Price);

                    //            arr1[i] += double.Parse(tongtien1.ToString());
                    //        }
                    //    }
                    //}
                }
                catch
                {

                }


              
                }

              
            
            return Json(new
            {
                arr,
                arr1
            });

            
        
        }
        public ActionResult Search(string keyword)
        {
            int totalRecord = 0;
            var model = new StatisticalDao().Search(keyword, ref totalRecord);
            ViewBag.Total = totalRecord;
            ViewBag.Keyword = keyword;
            return View(model);
        }
        [HttpGet]
        public ActionResult Export()
        {
            // Gọi lại hàm để tạo file excel
            var stream = new StatisticalDao().CreateExcelFile();
            // Tạo buffer memory strean để hứng file excel
            var buffer = stream as MemoryStream;
            // Đây là content Type dành cho file excel, còn rất nhiều content-type khác nhưng cái này mình thấy okay nhất
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
            // File name của Excel này là ExcelDemo
            Response.AddHeader("Content-Disposition", "attachment; filename=ExcelDemo.xlsx");
            // Lưu file excel của chúng ta như 1 mảng byte để trả về response
            Response.BinaryWrite(buffer.ToArray());
            // Send tất cả ouput bytes về phía clients
            Response.Flush();
            Response.End();
            // Redirect về luôn trang index <img draggable="false" role="img" class="emoji" alt="😀" src="https://s0.wp.com/wp-content/mu-plugins/wpcom-smileys/twemoji/2/svg/1f600.svg" scale="0">
            return RedirectToAction("Index");
        }
    }
}