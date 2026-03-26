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

        public async Task Execute(DTO.ConfirmTransactionDTO dto)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(dto.PlannedTransactioId);

            await _registerTransaction.Execute(new DTO.RegisterTransactionDTO()
            {
                Amount = plannedTransaction.Amount,
                Description = plannedTransaction.Description,
                Date = dto.Date,
                Type = plannedTransaction.Type,
                AccountId = plannedTransaction.AccountId,
                CategoryId = plannedTransaction.CategoryId,
                FamilyMemberId = plannedTransaction.FamilyMemberId

            });

            plannedTransaction.Status = PlannedTransactionStatus.Confirmed;
            await _plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
