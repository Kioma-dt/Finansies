using BuisnessLogic.Enums;

namespace BuisnessLogic.Entities
{
    public class BudgetFilter
    {
        public Guid Id { get; set; }
        public BudgetFilterType Type { get; set; }
        public string Value { get; set; } = String.Empty;

        public Guid BudgetId { get; set; }
        public Budget? Budget { get; set; }
    }
}
