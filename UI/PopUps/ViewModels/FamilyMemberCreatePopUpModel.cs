using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Commands;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace UI.PopUps.ViewModels
{
    public partial class FamilyMemberCreatePopUpModel
       : ObservableObject
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;

        public Action<CreateFamilyMemberCommand?>? CloseAction { get; set; }

        public FamilyMemberCreatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Name { get; set; }

        public async Task Initialize()
        { 
            Name = string.Empty;
        }

        [RelayCommand]
        public async Task Cancel()
        {
            CloseAction?.Invoke(null);
        }

        [RelayCommand]
        public async Task Create()
        {
            var name = Name;

            if (name is null)
            {
                throw new ArgumentException($"Enter Name!");
            }


            CloseAction?.Invoke(new CreateFamilyMemberCommand(
                _userContext.UserId,
                name
               ));
        }
    }
}
