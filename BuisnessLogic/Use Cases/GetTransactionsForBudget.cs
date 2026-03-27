using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;

namespace BuisnessLogic.Use_Cases
{
    public class GetTransactionsForBudget
    {
        ITransactionRepository _transactionRepository;
        IBudgetRepository _budgetRepository;

        public GetTransactionsForBudget(ITransactionRepository transactionRepository, IBudgetRepository budgetRepository)
        {
            _transactionRepository = transactionRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<IEnumerable<Transaction>> Execute(DTO.GetTransactionsForBudgetDTO dto)
        {
            var budget = await _budgetRepository.GetById(dto.BudgetId);

            var transactions = await _transactionRepository.GetWithFilters(budget.Filters);

            return transactions; 
        }
    }
}
