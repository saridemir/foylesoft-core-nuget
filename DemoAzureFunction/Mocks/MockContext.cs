using FoyleSoft.AzureCore.Implementations;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAzureFunction.Mocks
{
    public class MockContext : FoyleSoftMySqlContext
    {
        public MockContext(DbContextOptions<MockContext> options) : base(options)
        {
            //#if DEBUG
            Database.EnsureCreated();
            //#endif
        }
    }
}
