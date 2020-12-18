using Sample.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.DataAccess.Data.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
    }
}