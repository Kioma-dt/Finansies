using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.CategoryUseCasses.Commands
{
    public sealed record CreateCategoryCommand(Guid UserId,
        string Name,
        string Description,
        Guid? ParentId)
        : IRequest;

    public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        : IRequestHandler<CreateCategoryCommand>
    {
        public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request.ParentId is not null) 
            {
                var parent = await categoryRepository.GetById(request.UserId, request.ParentId.Value);

                if (parent is null)
                {
                    throw new ArgumentException($"No Parent with ID: {request.ParentId}");
                }
            }

            await categoryRepository.Add(new Category()
            {
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentId
            });
        }
    }
}
