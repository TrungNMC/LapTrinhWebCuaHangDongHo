using Model.EF;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class RatingDao
    {
        OnlineShopDbContext db = null;
        public RatingDao()
        {
            db = new OnlineShopDbContext();
        }
        public Rating ViewDetail(long id)
        {
            return db.Ratings.Find(id);
        }
        public List<RatingModel> ShowComment(long productId, ref int totalRecord, int page = 1, int pageSize = 3)
        {
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productId
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            totalRecord = model.Count();
            return model.OrderBy(x => x.CommentedOn).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        public float SumStar(long productid)
        {
            int sumstart = 0;
            float average = 0;
            int amount = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                sumstart += item.Rate;
                amount++;
            }
            if (amount == 0)
            {
                return 0;
            }
            else
            {
                average = sumstart / amount;
                return average;
            }
        }

        public List<StarModel> Star()
        {
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         select new
                         {
                             ProductId = a.ID,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn,
                         }).AsEnumerable().Select(x => new StarModel()
                         {
                             ProductId = x.ProductId,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            var model1 = new List<StarModel>();
            foreach (var item in model.ToList())
            {
                int cout = 0;
                foreach (var item1 in model1)
                {
                    if (item1.ProductId == item.ProductId)
                    {
                        item1.Star += item.Rate;
                        item1.amount++;
                        cout++;
                    }
                }
                if (cout == 0)
                {
                    model1.Add(item);
                }
            }
            var model2 = new List<StarModel>();
            foreach (var item1 in model1.ToList())
            {
                int dem = 0;
                foreach (var item2 in model2.ToList())
                {
                    if (item1.ProductId == item2.ProductId)
                    {
                        item2.SumStar = item1.Star / item1.amount;
                        dem++;
                    }
                }
                if (dem == 0)
                {
                    model2.Add(item1);
                }
            }
            model2.OrderByDescending(x => x.CommentedOn);
            return model2.ToList();
        }

        public float Sum5Star(long productid)
        {
            int sumstart = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 5)
                {
                    sumstart++;
                }
            }
            return sumstart;
        }
        public float Sum4Star(long productid)
        {
            int sumstart = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 4)
                {
                    sumstart++;
                }
            }
            return sumstart;
        }
        public float Sum3Star(long productid)
        {
            int sumstart = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 3)
                {
                    sumstart++;
                }
            }
            return sumstart;
        }
        public float Sum2Star(long productid)
        {
            int sumstart = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 2)
                {
                    sumstart++;
                }
            }
            return sumstart;
        }
        public float Sum1Star(long productid)
        {
            int sumstart = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 1)
                {
                    sumstart++;
                }
            }
            return sumstart;
        }
        public long Insert(Rating rating, long productId, long customerId)
        {
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productId && b.CustomerId == customerId
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new Rating()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn,
                         });
            rating.ProductId = productId;
            rating.CustomerId = customerId;
            rating.CommentedOn = DateTime.Now;
            db.Ratings.Add(rating);
            db.SaveChanges();
            return rating.ID;
        }
        public double Percent5Star(long productid)
        {
            double sumstart = 0;
            double average = 0;
            double amount = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 5)
                {
                    sumstart++;
                }
                amount++;
            }
            if (amount == 0)
            {
                return 0;
            }
            else
            {
                average = sumstart / amount;
                return average * (double)100;
            }
        }
        public double Percent4Star(long productid)
        {
            double sumstart = 0;
            double average = 0;
            double amount = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 4)
                {
                    sumstart++;
                }
                amount++;
            }
            if (amount == 0)
            {
                return 0;
            }
            else
            {
                average = sumstart / amount;
                return average * (double)100;
            }
        }
        public double Percent3Star(long productid)
        {
            double sumstart = 0;
            double average = 0;
            double amount = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 3)
                {
                    sumstart++;
                }
                amount++;
            }
            if (amount == 0)
            {
                return 0;
            }
            else
            {
                average = sumstart / amount;
                return average * (double)100;
            }
        }
        public double Percent2Star(long productid)
        {
            double sumstart = 0;
            double average = 0;
            double amount = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 2)
                {
                    sumstart++;
                }
                amount++;
            }
            if (amount == 0)
            {
                return 0;
            }
            else
            {
                average = sumstart / amount;
                return average * (double)100;
            }
        }
        public double Percent1Star(long productid)
        {
            double sumstart = 0;
            double average = 0;
            double amount = 0;
            var model = (from a in db.Products
                         join b in db.Ratings
                         on a.ID equals b.ProductId
                         join c in db.Users
                         on b.CustomerId equals c.ID
                         where b.ProductId == productid
                         select new
                         {
                             ProductId = a.ID,
                             CustomerId = c.ID,
                             CustomerName = c.Name,
                             Comment = b.Comment,
                             Rate = b.Rate,
                             CommentedOn = b.CommentedOn
                         }).AsEnumerable().Select(x => new RatingModel()
                         {
                             ProductId = x.ProductId,
                             CustomerId = x.CustomerId,
                             CustomerName = x.CustomerName,
                             Comment = x.Comment,
                             Rate = x.Rate,
                             CommentedOn = x.CommentedOn
                         });
            foreach (var item in model.ToList())
            {
                if (item.Rate == 1)
                {
                    sumstart++;
                }
                amount++;
            }
            if (amount == 0)
            {
                return 0;
            }
            else
            {
                average = sumstart / amount;
                return average * (double)100;
            }
        }
    }
}
