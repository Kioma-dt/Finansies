using DataAccess.RepositoriesImplementation;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DataAccess.Tests.GenericRepository
{
    public class RepositoryUpdateTests
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
        public async Task Update_ShouldModifyEntity_WhenExists()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entityId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Add(new TestEntity
                {
                    Id = entityId,
                    UserId = userId,
                    Name = "Old"
                });

                await context.SaveChangesAsync();
            }

            var updatedEntity = new TestEntity
            {
                Id = entityId,
                UserId = userId,
                Name = "New"
            };

            await repo.Update(updatedEntity);

            await using var verifyContext = CreateVerifyContext(dbName);

            var saved = await verifyContext.Set<TestEntity>()
                .FirstOrDefaultAsync(x => x.Id == entityId);

            saved.Should().NotBeNull();
            saved!.Name.Should().Be("New");
        }

        [Fact]
        public async Task Update_ShouldPersistAllFields()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entityId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Add(new TestEntity
                {
                    Id = entityId,
                    UserId = userId,
                    Name = "Old"
                });

                await context.SaveChangesAsync();
            }

            var updated = new TestEntity
            {
                Id = entityId,
                UserId = userId,
                Name = "Updated"
            };

            await repo.Update(updated);

            await using var verifyContext = CreateVerifyContext(dbName);

            var saved = await verifyContext.Set<TestEntity>()
                .FirstOrDefaultAsync(x => x.Id == entityId);

            saved!.UserId.Should().Be(userId);
            saved.Name.Should().Be("Updated");
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenEntityIsNull()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            Func<Task> act = () => repo.Update(null!);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Update_ShouldPersistChangesInDatabase()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entityId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Add(new TestEntity
                {
                    Id = entityId,
                    UserId = Guid.NewGuid(),
                    Name = "Old"
                });

                await context.SaveChangesAsync();
            }

            var updated = new TestEntity
            {
                Id = entityId,
                UserId = Guid.NewGuid(),
                Name = "Updated"
            };

            await repo.Update(updated);

            await using var verifyContext = CreateVerifyContext(dbName);

            var saved = await verifyContext.Set<TestEntity>()
                .FirstOrDefaultAsync(x => x.Id == entityId);

            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Updated");
        }

        [Fact]
        public async Task Update_ShouldDoNothing_WhenEntityDoesNotExist()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Untracked"
            };

            await repo.Update(entity);

            await using var verifyContext = CreateVerifyContext(dbName);

            var saved = await verifyContext.Set<TestEntity>()
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            saved.Should().BeNull();
        }

        [Fact]
        public async Task Update_ShouldOverrideExistingEntityCompletely()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entityId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Add(new TestEntity
                {
                    Id = entityId,
                    UserId = Guid.NewGuid(),
                    Name = "Old"
                });

                await context.SaveChangesAsync();
            }

            var updated = new TestEntity
            {
                Id = entityId,
                UserId = Guid.NewGuid(),
                Name = "CompletelyNew"
            };

            await repo.Update(updated);

            await using var verifyContext = CreateVerifyContext(dbName);

            var saved = await verifyContext.Set<TestEntity>()
                .FirstAsync(x => x.Id == entityId);

            saved.Name.Should().Be("CompletelyNew");
        }
    }
}
