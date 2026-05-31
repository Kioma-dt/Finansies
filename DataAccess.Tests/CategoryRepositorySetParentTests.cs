namespace DataAccess.Tests
{
    using BuisnessLogic.Entities;
    using DataAccess.RepositoriesImplementation;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class CategoryRepositorySetParentTests
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
        public async Task SetParent_ShouldSetParentId()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new CategoryRepository(factory);

            var userId = Guid.NewGuid();

            var categoryId = Guid.NewGuid();
            var parentId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Categories.AddRange(
                    new Category
                    {
                        Id = categoryId,
                        UserId = userId,
                        Name = "Child"
                    },
                    new Category
                    {
                        Id = parentId,
                        UserId = userId,
                        Name = "Parent"
                    });

                await context.SaveChangesAsync();
            }

            var parent = new Category
            {
                Id = parentId,
                UserId = userId
            };

            await repo.SetParent(userId, categoryId, parent);

            await using var verifyContext = CreateVerifyContext(dbName);

            var category = await verifyContext.Categories
                .FirstAsync(x => x.Id == categoryId);

            category.ParentId.Should().Be(parentId);
        }

        [Fact]
        public async Task SetParent_ShouldNotChangeParent_WhenCategoryDoesNotExist()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new CategoryRepository(factory);

            var parent = new Category
            {
                Id = Guid.NewGuid()
            };

            await repo.SetParent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                parent);

            await using var verifyContext = CreateVerifyContext(dbName);

            (await verifyContext.Categories.CountAsync())
                .Should()
                .Be(0);
        }

        [Fact]
        public async Task SetParent_ShouldNotChangeParent_WhenUserDoesNotOwnCategory()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new CategoryRepository(factory);

            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var categoryId = Guid.NewGuid();
            var parentId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Categories.Add(new Category
                {
                    Id = categoryId,
                    UserId = ownerId,
                    Name = "Category"
                });

                await context.SaveChangesAsync();
            }

            await repo.SetParent(
                anotherUserId,
                categoryId,
                new Category { Id = parentId });

            await using var verifyContext = CreateVerifyContext(dbName);

            var category = await verifyContext.Categories
                .FirstAsync(x => x.Id == categoryId);

            category.ParentId.Should().BeNull();
        }

        [Fact]
        public async Task SetParent_ShouldNotAllowSelfParenting()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new CategoryRepository(factory);

            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Categories.Add(new Category
                {
                    Id = categoryId,
                    UserId = userId
                });

                await context.SaveChangesAsync();
            }

            await repo.SetParent(
                userId,
                categoryId,
                new Category { Id = categoryId });

            await using var verifyContext = CreateVerifyContext(dbName);

            var category = await verifyContext.Categories
                .FirstAsync(x => x.Id == categoryId);

            category.ParentId.Should().BeNull();
        }

        [Fact]
        public async Task SetParent_ShouldReplaceExistingParent()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);
            var repo = new CategoryRepository(factory);

            var userId = Guid.NewGuid();

            var categoryId = Guid.NewGuid();
            var oldParentId = Guid.NewGuid();
            var newParentId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Categories.Add(new Category
                {
                    Id = categoryId,
                    UserId = userId,
                    ParentId = oldParentId
                });

                await context.SaveChangesAsync();
            }

            await repo.SetParent(
                userId,
                categoryId,
                new Category { Id = newParentId });

            await using var verifyContext = CreateVerifyContext(dbName);

            var category = await verifyContext.Categories
                .FirstAsync(x => x.Id == categoryId);

            category.ParentId.Should().Be(newParentId);
        }
    }
}
