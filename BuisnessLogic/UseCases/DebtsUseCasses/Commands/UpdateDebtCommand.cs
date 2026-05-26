using MediatR;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.UseCases.DebtsUseCasses.Commands
{
    public sealed record UpdateDebtCommand(Guid UserId,
        Guid Id,
        string Name,
        Guid? CategoryId,
        Guid? FamilyMemberId
        )
        : IRequest;

    public class UpdateDebtCommandHandler(IDebtRepository debtRepository)
        : IRequestHandler<UpdateDebtCommand>
    {
        public async Task Handle(UpdateDebtCommand request, CancellationToken cancellationToken)
        {
            var debt = await debtRepository.GetById(request.UserId, request.Id);
            if (debt is null)
            {
                throw new ArgumentException($"No Debt with Id: {request.Id}");
            }
            debt.Name = request.Name;
            debt.CategoryId = request.CategoryId;
            debt.FamilyMemberId = request.FamilyMemberId;
            await debtRepository.Update(debt);
        }
    }
}
