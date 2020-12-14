using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.DataAccess.Data.Repository.IRepository
{
    public interface ISP_Call : IDisposable
    {
        //usd dapper to pass parameters, ex. When you deleting a record, you need the id as parameter!

        //use ExecuteScalar returns an intger or boolean
        //first colum of first row in the result set
        T Single<T>(string procedureName, DynamicParameters param = null);

        //add, delete, or something not to retrieve anything
        void Execute(string procedureName, DynamicParameters param = null);

        //return a complete row
        T OneRecord<T>(string procedureName, DynamicParameters param = null);

        //return a list of record
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);

        //return two tables
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);
    }
}