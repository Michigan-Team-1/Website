using Team1.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Team1.DataSeed.Seeders
{
    public abstract class SeedBase
    {
        protected DataContext _context = null;
        protected DateTime _timestamp = DateTime.UtcNow;

        public SeedBase(DataContext context)
        {
            _context = context;
        }

        public abstract Task Seed();
    }
}
