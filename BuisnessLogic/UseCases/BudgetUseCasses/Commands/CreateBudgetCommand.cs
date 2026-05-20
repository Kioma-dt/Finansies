using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BuisnessLogic.UseCases.BudgetUseCasses.Commands
{
    public sealed record CreateBudgetCommand(Guid UserId,
            string Name,
            decimal Limit,
            DateTime StartDate,
            DateTime EndDate,
            List<(BudgetFilterType Type, string Value)> Filters)
        : IRequest;

    public class CreateBudgetCommandHandler(IMediator mediator, IBudgetRepository budgetRepository)
        : IRequestHandler<CreateBudgetCommand>
    {
        public async Task Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var budget = new Budget()
            {
                Id = id,
                Name = request.Name,
                Limit = request.Limit,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UserId = request.UserId
            };

            await budgetRepository.Add(budget);

            foreach (var (type, value) in request.Filters)
            {
                await mediator.Send(new AddFilterToBudgetCommand(request.UserId,
                    id,
                    type,
                    value));
            }
        }
    }
}
