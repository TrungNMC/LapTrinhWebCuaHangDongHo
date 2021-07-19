using Model.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class FeedbackDao
    {
        OnlineShopDbContext db = null;
        public FeedbackDao()
        {
            db = new OnlineShopDbContext();
        }
        public List<Feedback> ListAll()
        {
            return db.Feedbacks.Where(x => x.Status == true).OrderBy(y => y.CreatedDate).ToList();
        }
        public IEnumerable<Feedback> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Feedback> model = db.Feedbacks;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.Content.Contains(searchString) || x.Status.ToString().Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
        public long Insert(Feedback entity)
        {
            entity.CreatedDate = DateTime.Now;
            db.Feedbacks.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }
        public Feedback ViewDetail(int id)
        {
            return db.Feedbacks.Find(id);
        }
        public bool Update(Feedback entity)
        {
            try
            {
                var feedback = db.Feedbacks.Find(entity.ID);                
                feedback.Status = entity.Status;
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool Reply(Feedback entity)
        {
            try
            {
                var feedback = db.Feedbacks.Find(entity.ID);
                feedback.Reply = entity.Reply;
                feedback.Status = true;
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
                var feedback = db.Feedbacks.Find(id);
                db.Feedbacks.Remove(feedback);
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
            var feedback = db.Feedbacks.Find(id);
            feedback.Status = !feedback.Status;
            db.SaveChanges();
            return feedback.Status;
        }
        public int countfeedback()
        {
            var count = (from a in db.Feedbacks where a.Status == false select a).Count();
            return count;
        }
    }
}
