using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class FinansiesDbContextFactory : IDesignTimeDbContextFactory<FinansiesDbContext>
    {
        public FinansiesDbContext CreateDbContext(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<FinansiesDbContext>();

            optionsBuilder.UseMySql(
             "server=localhost;database=finansies_db;user=root;password=Kioma220;",
             new MySqlServerVersion(new Version(8, 0, 34))
         );

            return new FinansiesDbContext(optionsBuilder.Options);
        }
    }
}
