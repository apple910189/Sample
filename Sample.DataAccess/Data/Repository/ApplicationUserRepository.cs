using Sample.DataAccess.Data.Repository.IRepository;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.DataAccess.Data.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}