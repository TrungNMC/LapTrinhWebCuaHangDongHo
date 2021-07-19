using Model.EF;
using Model.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class StatisticalDao
    {
        OnlineShopDbContext db = null;
        public StatisticalDao()
        {
            db = new OnlineShopDbContext();
        }
        public List<StatisticalModel> listStatistical()
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            var model1 = new List<StatisticalModel>();
            foreach (var item in model.ToList())
            {
                int cout = 0;
                if (item.status == 1)
                {
                    foreach (var item1 in model1)
                    {
                        if (item1.date.Value.Month == item.date.Value.Month && item1.date.Value.Year == item.date.Value.Year)
                        {
                            item1.revenue += item.revenue;
                            item1.benefit += item.benefit;
                            cout++;

                        }
                    }
                    if (cout == 0)
                    {
                        model1.Add(item);
                    }
                }                
            }
            model1.OrderByDescending(x => x.date);
            return model1.ToList();
        }
        public List<StatisticalModel> Search(string keyword, ref int totalRecord)
        {
            totalRecord = db.Orders.Where(x => x.CreatedDate.ToString() == keyword).Count();
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            var model1 = new List<StatisticalModel>();
            foreach (var item in model.ToList())
            {
                int cout = 0;
                if (item.status == 1)
                {
                    foreach (var item1 in model1)
                    {
                        if (item1.date.Value.Month == item.date.Value.Month && item1.date.Value.Year == item.date.Value.Year)
                        {
                            item1.revenue += item.revenue;
                            item1.benefit += item.benefit;
                            cout++;
                        }
                    }
                    if (cout == 0)
                    {
                        model1.Add(item);
                    }
                }               
            }
            model1.OrderByDescending(x => x.date);
            return model1.ToList();
        }
        public Stream CreateExcelFile(Stream stream = null  )
        {
            var list = (from a in db.OrderDetails
                        join b in db.Orders on a.OrderID equals b.ID
                        join c in db.Products on a.ProductID equals c.ID
                        select new StatisticalModel()
                        {
                            date = b.CreatedDate,
                            revenue = a.Price * a.Quantity,
                            benefit = a.Quantity * (a.Price - c.OriginalPrice),
                            status = b.Status
                        }).AsEnumerable().Select(x => new StatisticalModel()
                        {
                            date = x.date,
                            revenue = x.revenue,
                            benefit = x.benefit,
                            
                        });
            var list1 = new List<StatisticalModel>();
            foreach (var item in list.ToList())
            {
                int cout = 0;
                if (item.status == 1)
                {
                    foreach (var item1 in list1)
                    {
                        if (item1.date.Value.Month == item.date.Value.Month && item1.date.Value.Year == item.date.Value.Year)
                        {
                            item1.revenue += item.revenue;
                            item1.benefit += item.benefit;
                            cout++;

                        }
                    }
                    if (cout == 0)
                    {
                        list1.Add(item);
                    }
                }               
            }
            list1.OrderByDescending(x => x.date);
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                // Tạo author cho file Excel
                excelPackage.Workbook.Properties.Author = "Shop online";
                // Tạo title cho file Excel
                excelPackage.Workbook.Properties.Title = "Thống kê doanh thu ";
                // Add Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("First Sheet");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var workSheet = excelPackage.Workbook.Worksheets[1];
                // Đổ data vào Excel file
                workSheet.Cells[1, 1].LoadFromCollection(list1, true, TableStyles.Dark9);
                BindingFormatForExcel(workSheet, list1);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        private void BindingFormatForExcel(ExcelWorksheet worksheet, List<StatisticalModel> listItems)
        {
            // Set default width cho tất cả column
            worksheet.DefaultColWidth = 10;
            // Tự động xuống hàng khi text quá dài
            worksheet.Cells.Style.WrapText = true;
            // Tạo header
            worksheet.Cells[1, 1].Value = "Thời gian";
            worksheet.Cells[1, 2].Value = "Doanh thu";
            worksheet.Cells[1, 3].Value = "Lợi nhuận";
            

            // Lấy range vào tạo format cho range đó ở đây là từ A1 tới D1
            using (var range = worksheet.Cells["A1:D1"])
            {
                // Set PatternType
                range.Style.Fill.PatternType = ExcelFillStyle.DarkGray;
                // Set Màu cho Background
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
                // Canh giữa cho các text
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                // Set Font cho text  trong Range hiện tại
                range.Style.Font.SetFromFont(new Font("Arial", 10));
                // Set Border
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                // Set màu ch Border
                range.Style.Border.Bottom.Color.SetColor(Color.Blue);
            }

            // Đỗ dữ liệu từ list vào 
            for (int i = 0; i < listItems.Count; i++)
            {
                var item = listItems[i];
                worksheet.Cells[i + 2, 1].Value = item.date.Value.Month +"-"+ item.date.Value.Year;
                worksheet.Cells[i + 2, 2].Value = item.revenue;
                worksheet.Cells[i + 2, 3].Value = item.benefit;
                worksheet.Cells[i + 2, 4].Value = item.status;
                // Format lại color nếu như thỏa điều kiện
                if (item.benefit > 1000000)
                {
                    // Ở đây chúng ta sẽ format lại theo dạng fromRow,fromCol,toRow,toCol
                    using (var range = worksheet.Cells[i + 2, 1, i + 2, 4])
                    {
                        // Format text đỏ và đậm
                        range.Style.Font.Color.SetColor(Color.Red);
                        range.Style.Font.Bold = true;
                    }
                }

            }
            // Format lại định dạng xuất ra ở cột Money
            worksheet.Cells[2, 4, listItems.Count + 4, 4].Style.Numberformat.Format = "#,## VNĐ";
            // fix lại width của column với minimum width là 15
            worksheet.Cells[1, 1, listItems.Count + 5, 4].AutoFitColumns(15);

            // Thực hiện tính theo formula trong excel
            // Hàm Sum 
            worksheet.Cells[listItems.Count + 3, 3].Value = "Tổng doanh thu :";
            worksheet.Cells[listItems.Count + 3, 4].Formula = "SUM(B2:B" + (listItems.Count + 1) + ")";
            worksheet.Cells[listItems.Count + 4, 3].Value = "Tổng lợi nhuận :";
            worksheet.Cells[listItems.Count + 4, 4].Formula = "SUM(C2:C" + (listItems.Count + 1) + ")";          
        }
        public decimal revenue5(DateTime? date)
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            decimal? revenue5 = 0;
            foreach (var item in model.ToList())
            {
                if (item.status == 4)
                {
                    if (item.date.Value.Year == date.Value.Year && item.date.Value.Month == date.Value.Month && item.date.Value.Day==date.Value.Day)
                    {
                        revenue5 += item.revenue;
                    }
                }
            }
            return (decimal)revenue5;
        }
        public decimal revenue6()
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            decimal? revenue6 = 0;
            foreach (var item in model.ToList())
            {
                if (item.status == 1)
                {
                    if (item.date.Value.Month == 6)
                    {
                        revenue6 += item.revenue;
                    }
                }
            }
            return (decimal)revenue6;
        }
        public decimal benefit5(DateTime? date)
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            decimal? benefit5 = 0;
            foreach (var item in model.ToList())
            {
                if (item.status == 4)
                {
                    if (item.date.Value.Year == date.Value.Year && item.date.Value.Month == date.Value.Month && item.date.Value.Day == date.Value.Day)
                    {
                        benefit5 += item.benefit;
                    }
                }
            }
            return (decimal)benefit5;
        }
        public decimal benefit6()
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            decimal? benefit6 = 0;
            foreach (var item in model.ToList())
            {
                if (item.status == 1)
                {
                    if (item.date.Value.Month == 6)
                    {
                        benefit6 += item.benefit;
                    }
                }
            }
            return (decimal)benefit6;
        }
        public decimal revenue4(int i)
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            decimal? revenue4 = 0;
            foreach (var item in model.Where(x=>x.status==4).ToList())
            {
                if (item.status == 4)
                {
                    if(item.date.Value.Month == i)
                    {
                        revenue4 += item.revenue;
                    }                   
                }
            }
            return (decimal)revenue4;
        }
        public decimal benefit4(int i)
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            decimal? benefit4 = 0;
            foreach (var item in model.ToList())
            {
                if (item.status == 4)
                {
                    if (item.date.Value.Month == i)
                    {
                        benefit4 += item.benefit;
                    }
                }
            }
            return (decimal)benefit4;
        }
        public decimal countrevenue()
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            decimal? totalrevenue = 0;
            foreach (var item in model.ToList().Where(x => x.date > DateTime.Now.AddDays(-1) && x.date <= DateTime.Now && x.status==4  ))
            {
                    totalrevenue += item.revenue;
            }
            return (decimal)totalrevenue;
        }
        public decimal countbenefit()
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new StatisticalModel()
                         {
                             date = b.CreatedDate,
                             revenue = a.Price * a.Quantity,
                             benefit = a.Quantity * (a.Price - c.OriginalPrice),
                             status = b.Status
                         }).AsEnumerable().Select(x => new StatisticalModel()
                         {
                             date = x.date,
                             revenue = x.revenue,
                             benefit = x.benefit,
                             status = x.status
                         });
            decimal? totalbenefit = 0;
            foreach (var item in model.ToList().Where(x=>x.date>DateTime.Now.AddDays(-1)&&x.date <= DateTime.Now && x.status == 4))
            {
              
                    totalbenefit += item.benefit;
                
            }
            return (decimal)totalbenefit;
        }
    }
}
