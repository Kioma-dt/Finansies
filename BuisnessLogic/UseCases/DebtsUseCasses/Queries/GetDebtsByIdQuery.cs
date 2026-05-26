using MediatR;
using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.UseCases.DebtsUseCasses.Queries
{
    public sealed record GetDebtsByIdQuery(Guid UserId, Guid Id)
       : IRequest<Debt>;

    public class GetDebtsByIdQueryHandler(IDebtRepository debtRepository)
        : IRequestHandler<GetDebtsByIdQuery, Debt>
    {
        public async Task<Debt> Handle(GetDebtsByIdQuery request, CancellationToken cancellationToken)
        {
            var debt = await debtRepository.GetById(request.UserId, request.Id);
            if (debt is null)
            {
                throw new ArgumentException($"No Debt with Id: {request.Id}");
            }
            return debt;
        }
    }
}
