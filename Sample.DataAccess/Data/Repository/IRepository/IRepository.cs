using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sample.DataAccess.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //Based on the id, we want to get the category from database
        T Get(int id);

        //get a list of category based on a number of requirements
        IEnumerable<T> GetAll(
            //in order to be genaric, we use Expression here.
            Expression<Func<T, bool>> filter = null,//you can get all the record by setting it as null
                                                    //we want orderby so we can use Func here.
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            //eager loading:when you have foreign key reference and if you load a product and you want to load the Category object of that product.
            string includeProperties = null
            );

        //this will return only one object
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
            );

        void Add(T entity);

        void Remove(int id);

        void Remove(T entity);

        //if you want to remove three categories all together
        void RemoveRange(IEnumerable<T> entity);
    }
}