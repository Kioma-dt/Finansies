using BuisnessLogic.Enums;
using BuisnessLogic.Filters;

namespace BuisnessLogic.Entities
{
    public class Budget
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Limit { get; set; } = 0;
        public DateTime UntilDate { get; set; }
        public List<IFilter> Filters { get; } = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
