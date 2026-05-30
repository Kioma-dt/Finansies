using DataAccess.RepositoriesImplementation;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DataAccess.Tests.GenericRepository
{
    public class RepositoryGetAllTests
    {
        private string CreateDbName() => Guid.NewGuid().ToString();

        private IDbContextFactory<FinansiesDbContext> CreateFactory(string dbName)
        {
            var options = new DbContextOptionsBuilder<FinansiesDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            var factory = new Mock<IDbContextFactory<FinansiesDbContext>>();

            factory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new TestFinansiesDbContext(options));

            return factory.Object;
        }

        private TestFinansiesDbContext CreateVerifyContext(string dbName)
        {
            return new TestFinansiesDbContext(
                new DbContextOptionsBuilder<FinansiesDbContext>()
                    .UseInMemoryDatabase(dbName)
                    .Options);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOnlyUserEntities()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.AddRange(
                    new TestEntity { Id = Guid.NewGuid(), UserId = userId, Name = "A" },
                    new TestEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "B" },
                    new TestEntity { Id = Guid.NewGuid(), UserId = userId, Name = "C" }
                );

                await context.SaveChangesAsync();
            }

            var result = await repo.GetAll(userId);

            result.Should().HaveCount(2);
            result.All(x => x.UserId == userId).Should().BeTrue();
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmpty_WhenNoEntitiesForUser()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var result = await repo.GetAll(Guid.NewGuid());

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllFields()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Add(new TestEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "FullEntity"
                });

                await context.SaveChangesAsync();
            }

            var result = await repo.GetAll(userId);

            result.Should().ContainSingle();
            result.First().Name.Should().Be("FullEntity");
        }
    }
}
