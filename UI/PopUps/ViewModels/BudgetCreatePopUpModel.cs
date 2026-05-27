using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using BuisnessLogic.UseCases.BudgetUseCasses.Commands;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Xml.XPath;

namespace UI.PopUps.ViewModels
{
    public class FilterItem
    {
        public BudgetFilterType Type { get; set; }
        public string? Name { get; set; }
        public string? SelectedValue { get; set; }
    }

    public partial class BudgetCreatePopUpModel
       : ObservableObject
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;

        public Action<CreateBudgetCommand?>? CloseAction { get; set; }

        public BudgetCreatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Name { get; set; }

        [ObservableProperty]
        public partial string? Limit { get; set; }

        [ObservableProperty]
        public partial DateTime StartDate {  get; set; }

        [ObservableProperty]
        public partial DateTime EndDate { get; set; }

        public ObservableCollection<Account> Accounts { get; } = new();

        [ObservableProperty]
        public partial Account? SelectedAccount {  get; set; }

        [ObservableProperty]
        public partial bool IsAccountFilter { get; set; } = false;



        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        [ObservableProperty]
        public partial bool IsCategoryFilter { get; set; } = false;


        public ObservableCollection<FamilyMember> FamilyMembers { get; } = new();

        [ObservableProperty]
        public partial FamilyMember? SelectedFamilyMember { get; set; }

        [ObservableProperty]
        public partial bool IsFamilyMemberFilter { get; set; } = false;


        public ObservableCollection<BudgetFilterType> FilterTypes { get; } = new() 
        {
            BudgetFilterType.Account,
            BudgetFilterType.Category,
            BudgetFilterType.FamilyMember
        };

        [ObservableProperty]
        public partial BudgetFilterType SelectedFilterType { get; set;  }


        public ObservableCollection<FilterItem> SelectedFilters { get; } = new();

        public ObservableCollection<TransactionType> TransactionTypes { get; } = new() { TransactionType.Expense, TransactionType.Income};

        [ObservableProperty]
        public partial TransactionType SelectedTrasactionType {  get; set; }

        public async Task Initialize()
        {
            Accounts.Clear();
            var accounts = await _mediator.Send(new GetAllAccountsQuery(_userContext.UserId));
            foreach (var account in accounts) 
            {
                Accounts.Add(account);
            }

            Categories.Clear();
            var categories = await _mediator.Send(new GetAllCategoriesQuery(_userContext.UserId));
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            FamilyMembers.Clear();
            var familyMembers = await _mediator.Send(new GetAllFamilyMembersQuery(_userContext.UserId));
            foreach (var familyMember in familyMembers)
            {
                FamilyMembers.Add(familyMember);
            }

            SelectedFilterType = BudgetFilterType.Account;
            SelectedAccount = Accounts.FirstOrDefault();
            SelectedCategory = null;
            SelectedFamilyMember = null;

            SelectedFilters.Clear();

            SelectedTrasactionType = TransactionType.Expense;

            Name = String.Empty;
            Limit = String.Empty;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddMonths(1);
        }

        [RelayCommand]
        public void ChangeSelectedFilter()
        {
            switch (SelectedFilterType)
            {
                case BudgetFilterType.Account:
                    {
                        SelectedCategory = null;
                        IsCategoryFilter = false;
                        SelectedFamilyMember = null;
                        IsFamilyMemberFilter = false;

                        SelectedAccount = Accounts.FirstOrDefault();
                        IsAccountFilter = true;
                        break;
                    }

                case BudgetFilterType.Category:
                    {
                        SelectedAccount = null;
                        IsAccountFilter = false;
                        SelectedFamilyMember = null;
                        IsFamilyMemberFilter = false;

                        SelectedCategory = Categories.FirstOrDefault();
                        IsCategoryFilter = true;
                        break;
                    }
                case BudgetFilterType.FamilyMember:
                    {
                        SelectedCategory = null;
                        IsCategoryFilter = false;
                        SelectedAccount = null;
                        IsAccountFilter = false;

                        SelectedFamilyMember = FamilyMembers.FirstOrDefault();
                        IsFamilyMemberFilter = true;
                        break;
                    }

            }
        }

        [RelayCommand]
        public void AddFilter()
        {
            switch (SelectedFilterType)
            {
                case BudgetFilterType.Account:
                    {
                        if (SelectedAccount is not null)
                        {
                            SelectedFilters.Add(new FilterItem()
                            {
                                Type = BudgetFilterType.Account,
                                Name = SelectedAccount.Name,
                                SelectedValue = SelectedAccount.Id.ToString(),
                            });
                        }
                        break;
                    }

                case BudgetFilterType.Category:
                    {
                        if (SelectedCategory is not null)
                        {
                            SelectedFilters.Add(new FilterItem()
                            {
                                Type = BudgetFilterType.Category,
                                Name = SelectedCategory.Name,
                                SelectedValue = SelectedCategory.Id.ToString(),
                            });
                        }
                        break;
                    }

                case BudgetFilterType.FamilyMember:
                    {
                        if (SelectedFamilyMember is not null)
                        {
                            SelectedFilters.Add(new FilterItem()
                            {
                                Type = BudgetFilterType.FamilyMember,
                                Name = SelectedFamilyMember.Name,
                                SelectedValue = SelectedFamilyMember.Id.ToString(),
                            });
                        }
                        break;
                    }
            }
        }

        [RelayCommand]
        public void AddAccountFilter()
        {
            if (SelectedAccount is not null)
            {
                SelectedFilters.Add(new FilterItem()
                {
                    Type = BudgetFilterType.Account,
                    Name = SelectedAccount.Name,
                    SelectedValue = SelectedAccount.Id.ToString(),
                });
            }
        }

        [RelayCommand]
        public void AddCategotyFilter()
        {
            if (SelectedCategory is not null)
            {
                SelectedFilters.Add(new FilterItem()
                {
                    Type = BudgetFilterType.Category,
                    Name = SelectedCategory.Name,
                    SelectedValue = SelectedCategory.Id.ToString(),
                });
            }
        }

        [RelayCommand]
        public void AddFamilyMemberFilter()
        {
            if (SelectedFamilyMember is not null)
            {
                SelectedFilters.Add(new FilterItem()
                {
                    Type = BudgetFilterType.FamilyMember,
                    Name = SelectedFamilyMember.Name,
                    SelectedValue = SelectedFamilyMember.Id.ToString(),
                });
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
                    throw new ArgumentException($"Enter Name!");
                }

                if (!decimal.TryParse(Limit, out var limit) || limit <= 0)
                {
                    throw new ArgumentException("Wrong Limit Format! (Decimal > 0)");
                }

                var startDate = StartDate;
                var endDate = EndDate;

                if (startDate > endDate)
                {
                    throw new ArgumentException("Start Date can't be later than End Date!");
                }

                var filters = new List<(BudgetFilterType Type, string Value)>();

                foreach (var filter in SelectedFilters)
                {
                    if (filter.SelectedValue is not null)
                    {
                        filters.Add((filter.Type, filter.SelectedValue));
                    }
                }


                CloseAction?.Invoke(new CreateBudgetCommand(
                    _userContext.UserId,
                    name,
                    limit,
                    startDate,
                    endDate,
                    filters));
            }
            catch (ArgumentException ex)
            {
                await Shell.Current.DisplayAlert(
                       "Can't Create Budget",
                       ex.Message,
                       "OK");
            }
        }
    }
}
