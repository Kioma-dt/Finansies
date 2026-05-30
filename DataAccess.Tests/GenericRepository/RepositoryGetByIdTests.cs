using DataAccess.RepositoriesImplementation;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DataAccess.Tests.GenericRepository
{
    public class RepositoryGetByIdTests
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
        public async Task GetById_ShouldReturnEntity_WhenExistsAndUserMatches()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var userId = Guid.NewGuid();
            var entityId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Add(new TestEntity
                {
                    Id = entityId,
                    UserId = userId,
                    Name = "Test"
                });

                await context.SaveChangesAsync();
            }

            var result = await repo.GetById(userId, entityId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(entityId);
            result.UserId.Should().Be(userId);
            result.Name.Should().Be("Test");
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var result = await repo.GetById(Guid.NewGuid(), Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenUserDoesNotMatch()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var realUserId = Guid.NewGuid();
            var wrongUserId = Guid.NewGuid();
            var entityId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Add(new TestEntity
                {
                    Id = entityId,
                    UserId = realUserId,
                    Name = "Secret"
                });

                await context.SaveChangesAsync();
            }

            var result = await repo.GetById(wrongUserId, entityId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenWrongIdButSameUser()
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
                    Name = "OnlyOne"
                });

                await context.SaveChangesAsync();
            }

            var result = await repo.GetById(userId, Guid.NewGuid());

            result.Should().BeNull();
        }
    }
}
