using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.BudgetUseCasses.Commands
{
    public sealed record AddFilterToBudgetCommand(Guid UserId,
        Guid BudgetId,
        BudgetFilterType Type,
        string Value)
        : IRequest;

    public class AddFilterToBudgetCommandHandler(IBudgetRepository budgetRepository)
        : IRequestHandler<AddFilterToBudgetCommand>
    {
        public async Task Handle(AddFilterToBudgetCommand request, CancellationToken cancellationToken)
        {
            var budget = await budgetRepository.GetById(request.UserId,
                request.BudgetId);

            if (budget is null)
            {
                throw new ArgumentException($"No Budget with Id: {request.BudgetId}");
            }

            var budgetFilter = new BudgetFilter() 
            { 
                BudgetId = request.BudgetId,
                Type = request.Type, 
                Value = request.Value
            };

            await budgetRepository.AddBudgetFilter(request.UserId, budgetFilter);
        }
    }
}
