using DataAccess.Enums;

namespace DataAccess.Entities
{
    public class Budget
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Limit { get; set; } = 0;
        public DateTime UntilDate { get; set; }
        public List<BudgetFilter> Filters { get; set; } = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
    public enum BudgetFilterType { FamilyMember}
    public class BudgetFilter
    {
        public Guid Id { get; set; }
        public BudgetFilterType Type { get; set; }

        public Guid BudgetId { get; set; }
        public Budget? Budget { get; set; };
    }


}
