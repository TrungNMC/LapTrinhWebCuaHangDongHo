using Model.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class AboutDao
    {
        OnlineShopDbContext db = null;
        public AboutDao()
        {
            db = new OnlineShopDbContext();
        }           
        public List<About> ListAll()
        {
            return db.Abouts.OrderBy(x => x.CreatedDate).ToList();
        }
        public About ViewDetail(long id)
        {
            return db.Abouts.Find(id);
        }
        public About GetByID(long id)
        {
            return db.Abouts.Find(id);
        }
        public IEnumerable<About> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<About> model = db.Abouts;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.Description.Contains(searchString) || x.Detail.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
        public IEnumerable<About> ListAllPage(int page, int pageSize)
        {
            IQueryable<About> model = db.Abouts;
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public long Insert(About entity)
        {
            entity.CreatedDate = DateTime.Now;
            db.Abouts.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }
        public bool Update(About entity)
        {
            try
            {
                var about = db.Abouts.Find(entity.ID);
                about.Name = entity.Name;
                about.MetaTitle = entity.MetaTitle;
                about.Image = entity.Image;
                about.Description = entity.Description;
                about.Detail = entity.Detail;
                about.ModifiedBy = entity.ModifiedBy;
                about.ModifiedDate = DateTime.Now;
                about.Status = entity.Status;
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
                var about = db.Abouts.Find(id);
                db.Abouts.Remove(about);
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
            var about = db.Abouts.Find(id);
            about.Status = !about.Status;
            db.SaveChanges();
            return about.Status;
        }
    }
}
