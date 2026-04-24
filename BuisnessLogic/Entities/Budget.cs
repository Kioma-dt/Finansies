using BuisnessLogic.Enums;

namespace BuisnessLogic.Entities
{
    public class Budget
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Limit { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<BudgetFilter> Filters { get; set; } = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }

}
