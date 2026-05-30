using DataAccess.RepositoriesImplementation;
using DataAccess.Tests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataAccess.Tests.GenericRepository
{

    public class RepositoryAddTests
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
        public async Task Add_ShouldAddEntity()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Test"
            };

            await repo.Add(entity);

            await using var verifyContext = CreateVerifyContext(dbName);

            var saved = await verifyContext.Set<TestEntity>().FirstOrDefaultAsync();

            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Test");
        }

        [Fact]
        public async Task Add_ShouldPersistUserId()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var userId = Guid.NewGuid();

            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "UserEntity"
            };

            await repo.Add(entity);

            await using var verifyContext = CreateVerifyContext(dbName);

            var saved = await verifyContext.Set<TestEntity>().FirstOrDefaultAsync();

            saved.Should().NotBeNull();
            saved!.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task Add_ShouldWork_WhenIdIsEmpty()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entity = new TestEntity
            {
                Id = Guid.Empty,
                UserId = Guid.NewGuid(),
                Name = "NoId"
            };

            await repo.Add(entity);

            await using var verifyContext = CreateVerifyContext(dbName);

            var saved = await verifyContext.Set<TestEntity>().FirstOrDefaultAsync();

            saved.Should().NotBeNull();
            saved!.Name.Should().Be("NoId");
        }

        [Fact]
        public async Task Add_ShouldThrow_WhenEntityIsNull()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            Func<Task> act = () => repo.Add(null!);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Add_ShouldAllowMultipleEntities()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            await repo.Add(new TestEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            });

            await repo.Add(new TestEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            });

            await using var verifyContext = CreateVerifyContext(dbName);

            var count = await verifyContext.Set<TestEntity>().CountAsync();

            count.Should().Be(2);
        }

        [Fact]
        public async Task Add_ShouldUseDbSet()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new Repository<TestEntity>(factory);

            var entity = new TestEntity
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            await repo.Add(entity);

            await using var verifyContext = CreateVerifyContext(dbName);

            var exists = await verifyContext.Set<TestEntity>()
                .AnyAsync(x => x.Id == entity.Id);

            exists.Should().BeTrue();
        }
    }
}
