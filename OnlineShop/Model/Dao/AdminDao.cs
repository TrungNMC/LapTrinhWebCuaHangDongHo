using Model.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class AdminDao
    {
        OnlineShopDbContext db = null;
        public AdminDao()
        {
            db = new OnlineShopDbContext();
        }
        public long Insert(Ad entity)
        {
            db.Admins.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }       
        public bool Update(Ad entity)
        {
            try
            {
                var admin = db.Admins.Find(entity.ID);           
                if (!string.IsNullOrEmpty(entity.Password))
                {
                    admin.Password = entity.Password;
                }
                admin.Email = entity.Email;
                admin.Name = entity.Name;
                admin.Status = entity.Status;
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                string s = e.Message.ToString() + "";
                return false;
            }
        }
        public bool ForgotPass(Ad entity)
        {
            try
            {
                var admin = db.Admins.Find(entity.ID);
                if (!string.IsNullOrEmpty(entity.Password))
                {
                    admin.Password = entity.Password;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                string s = e.Message.ToString() + "";
                return false;
            }
        }
        public IEnumerable<Ad> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Ad> model = db.Admins;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.UserName.Contains(searchString) || x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }
        public Ad GetById(string userName)
        {
            return db.Admins.SingleOrDefault(x => x.UserName == userName);
        }
        public Ad ViewDetail(int id)
        {
            return db.Admins.Find(id);
        }
        public int Login(string userName, string passWord)
        {
            var result = db.Admins.SingleOrDefault(x => x.UserName == userName);
            if (result == null)
            {
                return 0;
            }
            else
            {
                if (result.Password == passWord)
                {
                    if (result.Status == true)
                    {
                        return 1;
                    }
                    else return -1;
                }
                else return -2;
            }
        }       
        public List<UserGroup> ListAll()
        {
            return db.UserGroups.ToList();
        }
        public List<string> GetListCredential(string userName)
        {
            var admin = db.Admins.Single(x => x.UserName == userName);
            var data = (from a in db.Credentials
                        join b in db.UserGroups on a.UserGroupID equals b.ID
                        join c in db.Roles on a.RoleID equals c.ID
                        where b.ID == admin.GroupID
                        select new
                        {
                            RoleID = a.RoleID,
                            UserGroupID = a.UserGroupID
                        }).AsEnumerable().Select(x => new Credential()
                        {
                            RoleID = x.RoleID,
                            UserGroupID = x.UserGroupID
                        });
            return data.Select(x => x.RoleID).ToList();

        }
        public bool ChangeStatus(long id)
        {
            var admin = db.Admins.Find(id);
            admin.Status = !admin.Status;
            db.SaveChanges();
            return admin.Status;
        }
        public bool Delete(int id)
        {
            try
            {
                var admin = db.Admins.Find(id);
                db.Admins.Remove(admin);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool ckeckUserName(string userName)
        {
            return db.Admins.Count(x => x.UserName == userName) > 0;
        }
        public bool checkEmail(string email)
        {
            return db.Admins.Count(x => x.Email == email) > 0;
        }
        
    }
}
