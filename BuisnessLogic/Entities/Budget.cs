using BuisnessLogic.Enums;
using BuisnessLogic.Filters;

namespace BuisnessLogic.Entities
{
    public class Budget
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public decimal Limit { get; private set; } = 0;
        public TransactionType TransactionType {  get; private set; }
        public List<IFilter> Filters { get; } = new();

        public Guid UserId { get; private set; }
        public User? User { get; private set; }
    }
}
