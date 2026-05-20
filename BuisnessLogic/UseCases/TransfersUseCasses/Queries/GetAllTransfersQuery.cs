using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.TransfersUseCasses.Queries
{
    public sealed record GetAllTransfersQuery(Guid UserId)
        : IRequest<IEnumerable<Transfer>>;

    public class GetAllTransfersQueryHandler(ITransferRepository transferRepository)
        : IRequestHandler<GetAllTransfersQuery, IEnumerable<Transfer>>
    {
        public async Task<IEnumerable<Transfer>> Handle(GetAllTransfersQuery request, CancellationToken cancellationToken)
        {
            return await transferRepository.GetAll(request.UserId,
                x => x.FromAccount,
                x => x.ToAccount,
                x => x.User);
        }
    }
}
