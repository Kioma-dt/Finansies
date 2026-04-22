using BuisnessLogic.BudgetService;
using BuisnessLogic.Repositories;
using DataAccess.Entities;

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

        public async Task<IEnumerable<Transaction>> GetRelevantTransactions(Guid userId, Guid budgetId)
        {
            var budget = await _budgetRepository.GetById(userId, budgetId);

            var specification = _budgetExtender.GetFullExpression(budget);

            return await _transactionRepository.GetWithSpecification(userId, specification);
        }
    }
}
