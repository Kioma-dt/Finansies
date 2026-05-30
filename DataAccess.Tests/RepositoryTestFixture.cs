using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Moq;

    public class RepositoryFixture
    {
        public TestFinansiesDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<FinansiesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TestFinansiesDbContext(options);
        }

        public IDbContextFactory<FinansiesDbContext> CreateFactory(TestFinansiesDbContext context)
        {
            var factory = new Mock<IDbContextFactory<FinansiesDbContext>>();

            factory.Setup(x =>
                    x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(context);

            return factory.Object;
        }
    }
}
