using BuisnessLogic.Repositories;
using DataAccess.Entities;
using DataAccess.Enums;
using System.Dynamic;
using System.Linq.Expressions;


namespace BuisnessLogic.BudgetService
{
    public class BudgetRelevantTransactions
    {

        public BudgetRelevantTransactions(BudgetSpecificationsExtender budgetExtender, ITransactionRepository transactionRepository, IBudgetRepository budgetRepository)
        {
            _budgetExtender = budgetExtender;
            _transactionRepository = transactionRepository;
            _budgetRepository = budgetRepository;
        }

        public IEnumerable<Transaction> GetTransactions(Guid userId, Guid budgetId)
        {

        } 
    }
}
