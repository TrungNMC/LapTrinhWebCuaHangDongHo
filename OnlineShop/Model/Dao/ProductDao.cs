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
    public class ProductDao
    {
        OnlineShopDbContext db = null;
        public ProductDao()
        {
            db = new OnlineShopDbContext();
        }
        public List<Product> ListNewProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        public List<Product> ListPromotion()
        {
            return db.Products.Where(x => x.PromotionPrice != null).ToList();
        }
        public List<string> ListName(String keywork)
        {
            return db.Products.Where(x => x.Name.Contains(keywork)).Select(x => x.Name).ToList();
        }
        public List<Product> ListByCategoryId(long categoryID, ref int totalRecord, int page = 1, int pageSize = 8)
        {
            totalRecord = db.Products.Count();
            return db.Products.Where(x => x.CategoryID == categoryID).OrderBy(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        public List<ProductViewModel> Search(string keyword, ref int totalRecord, int page, int pageSize)
        {
            totalRecord = db.Products.Where(x => x.Name == keyword).Count();
            var model = (from a in db.Products
                         join b in db.ProductCategories
                         on a.CategoryID equals b.ID
                         where a.Name.Contains(keyword)
                         select new
                         {
                             CateMetaTitle = b.MetaTitle,
                             CateName = b.Name,
                             CreatedDate = a.CreatedDate,
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Price = a.Price,
                             PromotionPrice = a.PromotionPrice,
                             Status = a.Status
                         }).AsEnumerable().Select(x => new ProductViewModel()
                         {
                             CateMetaTitle = x.MetaTitle,
                             CateName = x.Name,
                             CreatedDate = x.CreatedDate,
                             ID = x.ID,
                             Images = x.Images,
                             Name = x.Name,
                             MetaTitle = x.MetaTitle,
                             Price = x.Price,
                             PromotionPrice = x.PromotionPrice,
                             Status = x.Status
                         });
            return model.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        public List<Product> SearchPrice(long pricemin, long pricemax, ref int totalRecord, int page, int pageSize)
        {
            totalRecord = db.Products.Where(x => x.Price > pricemin && x.Price < pricemax).Count();
            var s = db.Products.Where(x => x.Price > pricemin && x.Price < pricemax).OrderBy(x => x.CreatedDate).ToList();
            return s;
        }
        public List<Product> ListFeatureProduct(int top)
        {
            return db.Products.Where(x => x.TopHot != null && x.TopHot > DateTime.Now).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        public List<Product> ListRelatedProduct(long productId)
        {
            var product = db.Products.Find(productId);
            return db.Products.Where(x => x.ID != productId && x.CategoryID == product.CategoryID).ToList();
        }
        public List<Product> ListAll()
        {
            return db.Products.OrderBy(x => x.CreatedDate).ToList();
        }
        public Product ViewDetail(long id)
        {
            return db.Products.Find(id);
        }
        public IEnumerable<Product> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
        public List<Product> ProductPaging(ref int totalRecord, int page = 1, int pageSize = 9)
        {
            totalRecord = db.Products.Count();
            return db.Products.OrderBy(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        public Product GetByID(long id)
        {
            return db.Products.Find(id);
        }

        public long Insert(Product entity)
        {
            if (entity.Quantity > 0)
            {
                entity.Status = true;
            }
            else
            {
                entity.Status = false;
            }
            entity.CreatedDate = DateTime.Now;
            db.Products.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }
        public bool Update(Product entity)
        {
            try
            {
                var product = db.Products.Find(entity.ID);
                product.Name = entity.Name;
                product.Code = entity.Code;
                product.Image = entity.Image;
                product.MoreImages = entity.MoreImages;
                product.MetaTitle = entity.MetaTitle;
                product.Description = entity.Description;
                product.OriginalPrice = entity.OriginalPrice;
                product.Price = entity.Price;
                product.PromotionPrice = entity.PromotionPrice;
                product.Quantity = entity.Quantity;
                product.CategoryID = entity.CategoryID;
                product.ModifiedBy = entity.ModifiedBy;
                product.ModifiedDate = DateTime.Now;
                product.TopHot = entity.TopHot;
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool ChangeStatus(long id)
        {
            var product = db.Products.Find(id);
            product.Status = !product.Status;
            db.SaveChanges();
            return product.Status;
        }
    }
}
