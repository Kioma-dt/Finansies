using DataAccess.RepositoriesImplementation;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace DataAccess.Tests.GenericRepository
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class RepositoryDeleteTests
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
        public async Task Delete_ShouldRemoveEntityFromDatabase()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            await using (var setupContext = CreateVerifyContext(dbName))
            {
                var entity = new TestEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Name = "ToDelete"
                };

                setupContext.Add(entity);
                await setupContext.SaveChangesAsync();
            }

            await using var verifyContext1 = CreateVerifyContext(dbName);

            var entityToDelete = await verifyContext1.Set<TestEntity>()
                .FirstAsync();

            await repo.Delete(entityToDelete);

            await using var verifyContext2 = CreateVerifyContext(dbName);

            var exists = await verifyContext2.Set<TestEntity>()
                .AnyAsync(x => x.Id == entityToDelete.Id);

            exists.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_ShouldActuallyDeleteEntity()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            await using (var setupContext = CreateVerifyContext(dbName))
            {
                setupContext.Add(new TestEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid()
                });

                await setupContext.SaveChangesAsync();
            }

            await using var verifyContext1 = CreateVerifyContext(dbName);

            var entity = await verifyContext1.Set<TestEntity>().FirstAsync();

            await repo.Delete(entity);

            await using var verifyContext2 = CreateVerifyContext(dbName);

            var count = await verifyContext2.Set<TestEntity>().CountAsync();

            count.Should().Be(0);
        }

       

        [Fact]
        public async Task Delete_ShouldWork_WhenEntityIsNotTracked()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            await repo.Delete(entity);

            await using var verifyContext = CreateVerifyContext(dbName);

            var exists = await verifyContext.Set<TestEntity>()
                .AnyAsync(x => x.Id == entity.Id);

            exists.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_ShouldNotThrow_WhenEntityDoesNotExistInDb()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            Func<Task> act = () => repo.Delete(entity);

            await act.Should().ThrowAsync();
        }

        [Fact]
        public async Task Delete_ShouldNotDependOnUserId()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            await using (var setupContext = CreateVerifyContext(dbName))
            {
                setupContext.Add(new TestEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid()
                });

                await setupContext.SaveChangesAsync();
            }

            var entity = await CreateVerifyContext(dbName)
                .Set<TestEntity>()
                .FirstAsync();

            await repo.Delete(entity);

            await using var verifyContext = CreateVerifyContext(dbName);

            var result = await verifyContext.Set<TestEntity>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            result.Should().BeNull();
        }
    }
}
