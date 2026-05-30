using Microsoft.EntityFrameworkCore;


namespace DataAccess.Tests
{
    public class TestFinansiesDbContext : FinansiesDbContext
    {
        public TestFinansiesDbContext(DbContextOptions<FinansiesDbContext> options)
            : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities => Set<TestEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestEntity>();
        }
    }
}
