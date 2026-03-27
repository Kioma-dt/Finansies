using BuisnessLogic.Repositories;
using System.Runtime.CompilerServices;

namespace BuisnessLogic.Use_Cases
{
    public class UpdatePlannedTransaction
    {
        readonly IPlannedTransactionRepository _plannedTransactionRepository;
        
        public UpdatePlannedTransaction(IPlannedTransactionRepository plannedTransactionRepository)
        {
            _plannedTransactionRepository = plannedTransactionRepository;
        }

        public async Task Execute(DTO.UpdatePlanedTransactionDTO dto)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(dto.PlannedTransactionId);

            plannedTransaction.Update(dto.Date);

            await _plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
