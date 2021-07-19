using Model.EF;
using Model.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class OrderDao
    {
        OnlineShopDbContext db = null;
        public OrderDao()
        {
            db = new OnlineShopDbContext();
        }
        public long Insert(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();
            return order.ID;
        }
        public List<OrderViewModel> ListOrder()
        {
            var model = (from a in db.OrderDetails
                         join b in db.Orders on a.OrderID equals b.ID
                         join c in db.Products on a.ProductID equals c.ID
                         select new OrderViewModel()
                         {
                             OrderId = b.ID,
                             ProductId = a.ProductID,
                             ProductName = c.Name,
                             CreatedDate = b.CreatedDate,
                             ShipName = b.ShipName,
                             ShipAddress = b.ShipAddress,
                             ShipEmail = b.ShipEmail,
                             Status = b.Status,
                             Quantity = a.Quantity,
                             Price = a.Price * a.Quantity,
                         }).AsEnumerable().Select(x => new OrderViewModel()
                         {
                             OrderId = x.OrderId,
                             ProductId = x.ProductId,
                             ProductName = x.ProductName,
                             CreatedDate = x.CreatedDate,
                             ShipName = x.ShipName,
                             ShipAddress = x.ShipAddress,
                             ShipEmail = x.ShipEmail,
                             Status = x.Status,
                             Quantity = x.Quantity,
                             Price = x.Price
                         });
            model.OrderByDescending(x => x.CreatedDate);
            return model.ToList();
        }
        public bool ChangeStatus(long id)
        {
            var order = db.Orders.Find(id);
            order.Status = 0;
            db.SaveChanges();
            return true;
        }
        public List<Order> ListByOrderId(long orderID)
        {
            return db.Orders.Where(x => x.ID == orderID).ToList();
        }
        public Order ViewDetail(long id)
        {
            return db.Orders.Find(id);
        }
        public List<string> ListName(String keywork)
        {
            return db.Orders.Where(x => x.ShipName.Contains(keywork)).Select(x => x.ShipName).ToList();
        }
        public IEnumerable<Order> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Order> model = db.Orders.Where(x=>x.Status<4);
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => (x.ShipName.Contains(searchString) || x.ShipAddress.Contains(searchString) || x.ShipMobile.Contains(searchString)) && x.Status<4);
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
        public bool Update(Order entity)
        {
            try
            {
                var order = db.Orders.Find(entity.ID);
                order.CustomerID = entity.CustomerID;
                order.ShipName = entity.ShipName;
                order.ShipMobile = entity.ShipMobile;
                order.ShipAddress = entity.ShipAddress;
                order.ShipEmail = entity.ShipEmail;
                order.Status = entity.Status;
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool Delete(long id)
        {
            try
            {
                var order = db.Orders.Find(id);
                var orderdetail = db.OrderDetails.Where(x => x.OrderID == id).ToList();
                foreach (var item in orderdetail)
                {
                    db.OrderDetails.Remove(item);
                    db.SaveChanges();
                }
                db.Orders.Remove(order);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public int counttask()
        {
            var count = (from a in db.Orders where a.Status == 0 select a).Count();
            return count;
        }     
    }
}
