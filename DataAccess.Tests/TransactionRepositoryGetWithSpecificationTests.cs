namespace DataAccess.Tests
{
    using BuisnessLogic.Entities;
    using DataAccess.RepositoriesImplementation;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class TransactionRepositoryGetWithSpecificationTests
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

        private sealed record SeedData(
            Account Account,
            Category Category,
            FamilyMember FamilyMember,
            Debt Debt,
            User User,
            TransactionTag Tag);

        private async Task<SeedData> SeedRequiredEntities(
            FinansiesDbContext context,
            Guid userId)
        {
            var user = new User
            {
                Id = userId,
                Name = "User",
                PasswordHash = "Hash"
            };

            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Account"
            };

            var category = new Category
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Category"
            };

            var familyMember = new FamilyMember
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Member"
            };

            var debt = new Debt
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Debt"
            };

            var tag = new TransactionTag
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Tag"
            };

            context.Users.Add(user);
            context.Accounts.Add(account);
            context.Categories.Add(category);
            context.FamilyMembers.Add(familyMember);
            context.Debts.Add(debt);
            context.TransactionTags.Add(tag);

            await context.SaveChangesAsync();

            return new SeedData(account, category, familyMember, debt, user, tag);
        }

        private Transaction CreateTransactionWithRelations(
            Guid userId,
            decimal amount,
            Account account,
            Category category,
            FamilyMember familyMember,
            Debt debt,
            User user,
            TransactionTag tag)
        {
            return new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,

                AccountId = account.Id,
                Account = account,

                CategoryId = category.Id,
                Category = category,

                FamilyMemberId = familyMember.Id,
                FamilyMember = familyMember,

                DebtId = debt.Id,
                Debt = debt,

                User = user,

                TransactionTags = new List<TransactionTag>
            {
                tag
            }
            };
        }

        [Fact]
        public async Task GetWithSpecification_ShouldReturnMatchingTransactions()
        {
            var dbName = CreateDbName();
            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                var data = await SeedRequiredEntities(context, userId);

                context.Transactions.AddRange(
                    CreateTransactionWithRelations(userId, 100, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag),
                    CreateTransactionWithRelations(userId, 200, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag));

                await context.SaveChangesAsync();
            }

            var repo = new TransactionRepository(CreateFactory(dbName));

            var result = await repo.GetWithSpecification(
                userId,
                x => x.Amount >= 150);

            result.Should().HaveCount(1);
            result[0].Amount.Should().Be(200);
        }

        [Fact]
        public async Task GetWithSpecification_ShouldFilterByUserId()
        {
            var dbName = CreateDbName();

            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                var data1 = await SeedRequiredEntities(context, user1);
                var data2 = await SeedRequiredEntities(context, user2);

                context.Transactions.AddRange(
                    CreateTransactionWithRelations(user1, 100, data1.Account, data1.Category, data1.FamilyMember, data1.Debt, data1.User, data1.Tag),
                    CreateTransactionWithRelations(user2, 100, data2.Account, data2.Category, data2.FamilyMember, data2.Debt, data2.User, data2.Tag));

                await context.SaveChangesAsync();
            }

            var repo = new TransactionRepository(CreateFactory(dbName));

            var result = await repo.GetWithSpecification(user1, x => true);

            result.Should().HaveCount(1);
            result[0].UserId.Should().Be(user1);
        }

        [Fact]
        public async Task GetWithSpecification_ShouldReturnEmptyList_WhenNoMatches()
        {
            var dbName = CreateDbName();
            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                var data = await SeedRequiredEntities(context, userId);

                context.Transactions.Add(
                    CreateTransactionWithRelations(userId, 100, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag));

                await context.SaveChangesAsync();
            }

            var repo = new TransactionRepository(CreateFactory(dbName));

            var result = await repo.GetWithSpecification(
                userId,
                x => x.Amount > 1000);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetWithSpecification_ShouldReturnAllUserTransactions_WhenSpecificationAlwaysTrue()
        {
            var dbName = CreateDbName();
            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                var data = await SeedRequiredEntities(context, userId);

                context.Transactions.AddRange(
                    CreateTransactionWithRelations(userId, 100, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag),
                    CreateTransactionWithRelations(userId, 200, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag));

                await context.SaveChangesAsync();
            }

            var repo = new TransactionRepository(CreateFactory(dbName));

            var result = await repo.GetWithSpecification(userId, x => true);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetWithSpecification_ShouldReturnEmptyList_WhenUserHasNoTransactions()
        {
            var dbName = CreateDbName();

            await using (var context = CreateVerifyContext(dbName))
            {
                var otherUser = Guid.NewGuid();
                var data = await SeedRequiredEntities(context, otherUser);

                context.Transactions.Add(
                    CreateTransactionWithRelations(otherUser, 100, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag));

                await context.SaveChangesAsync();
            }

            var repo = new TransactionRepository(CreateFactory(dbName));

            var result = await repo.GetWithSpecification(Guid.NewGuid(), x => true);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetWithSpecification_ShouldLoadNavigationProperties()
        {
            var dbName = CreateDbName();
            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                var data = await SeedRequiredEntities(context, userId);

                context.Transactions.Add(
                    CreateTransactionWithRelations(userId, 100, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag));

                await context.SaveChangesAsync();
            }

            var repo = new TransactionRepository(CreateFactory(dbName));

            var result = await repo.GetWithSpecification(userId, x => true);

            result.Should().ContainSingle();

            var t = result.Single();

            t.Account.Should().NotBeNull();
            t.Category.Should().NotBeNull();
            t.FamilyMember.Should().NotBeNull();
            t.Debt.Should().NotBeNull();
            t.User.Should().NotBeNull();
            t.TransactionTags.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetWithSpecification_ShouldApplyComplexSpecification()
        {
            var dbName = CreateDbName();
            var userId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                var data = await SeedRequiredEntities(context, userId);

                context.Transactions.AddRange(
                    CreateTransactionWithRelations(userId, 50, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag),
                    CreateTransactionWithRelations(userId, 150, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag),
                    CreateTransactionWithRelations(userId, 250, data.Account, data.Category, data.FamilyMember, data.Debt, data.User, data.Tag));

                await context.SaveChangesAsync();
            }

            var repo = new TransactionRepository(CreateFactory(dbName));

            var result = await repo.GetWithSpecification(
                userId,
                x => x.Amount >= 100 && x.Amount <= 200);

            result.Should().ContainSingle();
            result[0].Amount.Should().Be(150);
        }
    }
}
