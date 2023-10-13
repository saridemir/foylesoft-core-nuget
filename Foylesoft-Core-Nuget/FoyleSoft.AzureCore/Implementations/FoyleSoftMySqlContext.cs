using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public class FoyleSoftMySqlContext : FoyleSoftContext
    {
        public FoyleSoftMySqlContext(DbContextOptions options, bool hasDefaultTable = true)
: base(options, hasDefaultTable)
        {

        }
        public override void CreateModel<T>(ModelBuilder modelBuilder, string tableName)
        {
            modelBuilder.Entity<T>().ToTable(tableName);
        }

    }
}
