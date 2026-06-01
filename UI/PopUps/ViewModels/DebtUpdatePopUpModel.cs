using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UI.PopUps.Abstraction;

namespace UI.PopUps.ViewModels
{
    public sealed record DebtUpdatePopUpModelParameters(Guid Id,
            string Name,
            Guid? CategoryId,
            Guid? FamilyMemberId)
    {
    }
    public partial class DebtUpdatePopUpModel
        : ObservableObject,
        IPopUpInitializable<DebtUpdatePopUpModelParameters>
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;
        Guid _debtId;

        public Action<UpdateDebtCommand?>? CloseAction { get; set; }

        public DebtUpdatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Name { get; set; }

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        public ObservableCollection<FamilyMember> FamilyMembers { get; } = new();

        [ObservableProperty]
        public partial FamilyMember? SelectedFamilyMember { get; set; }

        public async Task Initialize(DebtUpdatePopUpModelParameters parameters)
        {
            _debtId = parameters.Id;

            Name = parameters.Name;

            Categories.Clear();

            Categories.Insert(0, new Category() { Id = Guid.Empty, Name = "-No Category-" });

            var categories = await _mediator.Send(new GetAllCategoriesQuery(_userContext.UserId));

            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            if (parameters.CategoryId is null)
            {
                SelectedCategory = Categories.FirstOrDefault();

            }
            else
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == parameters.CategoryId);
            }


            FamilyMembers.Clear();

            FamilyMembers.Insert(0, new FamilyMember() { Id = Guid.Empty, Name = "-No Family Member-" });

            var familyMembers = await _mediator.Send(new GetAllFamilyMembersQuery(_userContext.UserId));

            foreach (var familyMember in familyMembers)
            {
                FamilyMembers.Add(familyMember);
            }

            if (parameters.FamilyMemberId is null)
            {
                SelectedFamilyMember = FamilyMembers.FirstOrDefault();
            }
            else
            {
                SelectedFamilyMember = FamilyMembers.FirstOrDefault(f => f.Id == parameters.FamilyMemberId);
            }
        }

        [RelayCommand]
        public async Task Cancel()
        {
            CloseAction?.Invoke(null);
        }

        [RelayCommand]
        public async Task Create()
        {
            try
            {
                var name = Name;

                if (String.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Enter Name!");
                }

                var category = SelectedCategory;
                var family = SelectedFamilyMember;

                var comand = new UpdateDebtCommand
                    (
                        _userContext.UserId,
                        _debtId,
                        name,
                        category?.Id == Guid.Empty ? null : category?.Id,
                        family?.Id == Guid.Empty ? null : family?.Id
                    );

                CloseAction?.Invoke(comand);
            }
            catch (ArgumentException ex)
            {
                if (Shell.Current is not null)
                {
                    await Shell.Current.DisplayAlert(
                       "Can't Update Debt",
                       ex.Message,
                       "OK");
                }
            }

            
        }
    }
}
