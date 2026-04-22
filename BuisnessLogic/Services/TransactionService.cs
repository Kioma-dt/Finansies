using BuisnessLogic.Repositories;
using DataAccess.Entities;

namespace BuisnessLogic.Services
{
    public interface ITrasnsactionService
    {
        Task RegsiterTransaction(decimal amount,
            string description);
    }
    public class TransactionService
    {
        readonly ITransactionRepository _transactionRepository;
        readonly IAccountRepository _accountRepository;
        readonly ICategoryRepository _categoryRepository;
        readonly IFamilyMemberRepository _familyMemberRepository;
        readonly ITransactionTagRepository _transactionTagRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository, IFamilyMemberRepository familyMemberRepository, ITransactionTagRepository transactionTagRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _familyMemberRepository = familyMemberRepository;
            _transactionTagRepository = transactionTagRepository;
        }

        
    }
}
