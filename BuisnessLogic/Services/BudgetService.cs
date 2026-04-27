using BuisnessLogic.BudgetService;
using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;

namespace BuisnessLogic.Services
{
    public interface IBudgetService
    {
        Task<IEnumerable<Transaction>> GetRelevantTransactions(Guid userId,
            Guid budgetId);
    }
    public class BudgetService : IBudgetService
    {
        readonly IBudgetSpecificationsExtender _budgetExtender;
        readonly ITransactionRepository _transactionRepository;
        readonly IBudgetRepository _budgetRepository;

        public BudgetService(IBudgetSpecificationsExtender budgetExtender, ITransactionRepository transactionRepository, IBudgetRepository budgetRepository)
        {
            _budgetExtender = budgetExtender;
            _transactionRepository = transactionRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task AddFilter(Guid userId, Guid budgetId, BudgetFilterType type, string value)
        {
            var budget = await _budgetRepository.GetById(userId, budgetId);

            if (budget is null)
            {
                throw new ArgumentException($"No Budget with Id: {budgetId}");
            }

            var budgetFilter = new BudgetFilter() { BudgetId = budgetId, Type = type, Value = value };

            await _budgetRepository.AddBudgetFilter(userId, budgetId, budgetFilter);
        }

        public async Task<IEnumerable<Transaction>> GetRelevantTransactions(Guid userId, Guid budgetId)
        {
            var budget = await _budgetRepository.GetById(userId, budgetId);

            if (budget is null)
            {
                throw new ArgumentException($"No Budget with Id: {budgetId}");
            }

            var specification = _budgetExtender.GetFullExpression(budget);

            return await _transactionRepository.GetWithSpecification(userId, specification) ?? new List<Transaction>();
        }
    }
}
