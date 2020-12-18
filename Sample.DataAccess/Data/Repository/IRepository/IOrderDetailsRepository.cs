using Sample.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.DataAccess.Data.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails obj);
    }
}