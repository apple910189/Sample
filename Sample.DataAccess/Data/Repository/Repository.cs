using Microsoft.EntityFrameworkCore;
using Sample.DataAccess.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sample.DataAccess.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        //for all of the Repository methods, we have to modify databse.
        //whenever we have to modify database, we need DbContext and DbSet
        //we can use ApplicationDbContext instead of DbContext because it extends from DbContext
        //we need to get instance of this _db using constructor and dependency injection
        private readonly ApplicationDbContext _db;

        internal DbSet<T> dbSet;

        //we need to get the ApplicationDbContext using constructor and dependency injection
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
            //so this way we will have the dbSet, and we'll modify the dbSet directly.
        }

        public void Add(T entity)
        {
            // all we have to do is on out dbSet, we have to add the context.
            // so we will add the entity here.
            dbSet.Add(entity);
        }

        public T Get(int id)
        {
            // use dbSet and find the id.
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            //eager loading means if you have a product, inside the product you have a category id.
            //And the category id is foreign key reference to the category id inside your category.
            //That way you will be linking a product to a category.
            //So when you load the product, you also want to load the Category object.
            //That way, based on that id, you can display the category name.
            //And you want to do that in a single load.
            if (includeProperties != null)
            {
                //you have other features that you want to load from different tables.
                //So all of the table names, we want to pass here in a string seperated by comma
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    //And then for each of those items or includeProp, we would include that in query.
                    query = query.Include(includeProp);
                }
            }

            if (orderBy != null)
            {
                //we will order this by qurey.tolist
                return orderBy(query).ToList();
            }
            //return the IEnumerable based on all the conditions that were passed
            return query.ToList();
        }

        //Do similar as GetAll without orderBy
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            //return only one record
            return query.FirstOrDefault();
        }

        public void Remove(int id)
        {
            //find the entity based on the id
            T entity = dbSet.Find(id);
            //then remove that entity
            Remove(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}