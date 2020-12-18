using Sample.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.DataAccess.Data.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        //void Update(ApplicationUser category);
    }
}