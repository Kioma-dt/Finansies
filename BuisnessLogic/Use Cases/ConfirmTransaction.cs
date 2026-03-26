using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.Use_Cases
{
    public class ConfirmTransaction
    {
        readonly IRegisterTransaction _registerTransaction;
        readonly IPlannedTransactionRepository _plannedTransactionRepository;

        public ConfirmTransaction(IRegisterTransaction registerTransaction, IPlannedTransactionRepository plannedTransactionRepository)
        {
            _plannedTransactionRepository = plannedTransactionRepository;
            _registerTransaction = registerTransaction;
        }

        public async Task Execute(Guid plannedTrabsactionId, DateTime date)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(plannedTrabsactionId);

            await _registerTransaction.Execute(new DTO.RegisterTransactionDTO
            {
                Amount = plannedTransaction.Amount,
                Description = plannedTransaction.Description,
                Date = date,
                Type = plannedTransaction.Type,
                AccountId = plannedTransaction.AccountId,
                CategoryId = plannedTransaction.CategoryId,
                FamilyMemberId = plannedTransaction.FamilyMemberId

            });
        }
    }
}
