using FoyleSoft.AzureCore.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAzureFunction.Mocks
{
    public class MockLogContext : FoyleSoftMySqlLogContext
    {
        public MockLogContext(DbContextOptions<MockLogContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
