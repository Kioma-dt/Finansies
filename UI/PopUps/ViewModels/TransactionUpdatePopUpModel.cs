using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.DebtsUseCasses.Queries;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UI.PopUps.Abstraction;

namespace UI.PopUps.ViewModels
{
    public sealed record TransactionUpdatePopUpModelParameters(Guid TransactionId,
        string? Description,
        DateTime Date,
        Guid? CategoryId,
        Guid? FamilyMemberId)
    {
    }
    public partial class TransactionUpdatePopUpModel
        : ObservableObject,
        IPopUpInitializable<TransactionUpdatePopUpModelParameters>
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;
        
        Guid _transactionId;

        public Action<UpdateTransactionCommand?>? CloseAction { get; set; }

        public TransactionUpdatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Description { get; set; }


        [ObservableProperty]
        public partial DateTime Date { get; set; }

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        public ObservableCollection<FamilyMember> FamilyMembers { get; } = new();

        [ObservableProperty]
        public partial FamilyMember? SelectedFamilyMember { get; set; }


        public async Task Initialize(TransactionUpdatePopUpModelParameters parameters)
        {
            _transactionId = parameters.TransactionId;

            Description = parameters.Description;

            Date = parameters.Date;

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
                SelectedCategory = Categories.FirstOrDefault(x => x.Id == parameters.CategoryId);
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
                SelectedFamilyMember = FamilyMembers.FirstOrDefault(x => x.Id == parameters.FamilyMemberId);
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
                var description = Description;

                if (String.IsNullOrWhiteSpace(description))
                {
                    throw new ArgumentException("Enter Description!");
                }

                var date = Date;

                var category = SelectedCategory;
                var family = SelectedFamilyMember;

                var comand = new UpdateTransactionCommand
                    (
                        _userContext.UserId,
                        _transactionId,
                        description,
                        date,
                        category?.Id == Guid.Empty ? null : category?.Id,
                        family?.Id == Guid.Empty ? null : family?.Id
                    );

                CloseAction?.Invoke(comand);
            }
            catch (ArgumentException ex)
            {
                await Shell.Current.DisplayAlert(
                       "Can't Update Transaction",
                       ex.Message,
                       "OK");
            }
            
        }
    }
}
