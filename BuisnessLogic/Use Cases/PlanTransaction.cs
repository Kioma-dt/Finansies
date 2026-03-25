using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.Use_Cases.DTO;

namespace BuisnessLogic.Use_Cases
{
    public class PlanTransaction
    {
        readonly IPlannedTransactionRepository _plannedTransactionRepository;

        public PlanTransaction(IPlannedTransactionRepository plannedTransactionRepository)
        {
            _plannedTransactionRepository = plannedTransactionRepository;
        }

        public async Task Execute(PlanTransactionDTO dto)
        {
            var planedTransaction = new PlannedTransaction()
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Type = dto.Type,
                PlannedDate = dto.PlannedDate,
                AccountId = dto.AccountId,
                CategoryId = dto.CategoryId,
                FamilyMemberId = dto.FamilyMemberId
            };

            await _plannedTransactionRepository.Add(planedTransaction);
        }
    }
}
