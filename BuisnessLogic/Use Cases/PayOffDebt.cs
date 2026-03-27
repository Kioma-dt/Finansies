using BuisnessLogic.Repositories;

namespace BuisnessLogic.Use_Cases
{
    public class PayOffDebt
    {
        readonly IDebtRepository _debtRepository;

        public PayOffDebt(IDebtRepository debtRepository)
        {
            _debtRepository = debtRepository;
        }

        public async Task Execute(DTO.PayOffDebtDTO dto)
        {
            var debt = await _debtRepository.GetById(dto.DebtId);

            debt.MakeAPayment(dto.Amount, dto.Date);

            await _debtRepository.Update(debt);
        }
    }
}
