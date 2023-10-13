using Microsoft.EntityFrameworkCore;

namespace FoyleSoft.AzureCore.Implementations
{
    public class FoyleSoftMySqlLogContext : FoyleSoftLogContext
    {
        public FoyleSoftMySqlLogContext(DbContextOptions options, bool hasDefaultTable = true)
                : base(options, hasDefaultTable)
        {

        }
    }
}
