using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;
using DataAccess.RepositoriesImplementation;
using System.Collections.Generic;

namespace UI.Popups;

public class BudgetCreateDTO(string Name,
    decimal Limit,
    DateTime StartDate,
    DateTime EndDate,
    List<(BudgetFilterType Type, string Value)> Filters)
{
    public string Name { get; } = Name;
    public decimal Limit { get; } = Limit;
    public DateTime StartDate { get; } = StartDate;
    public DateTime EndDate { get; } = EndDate;
    public List<(BudgetFilterType Type, string Value)> Filters { get; } = Filters;
}
    

public class FilterItem
{
    public BudgetFilterType Type { get; set; }
    public string? SelectedValue { get; set; }
}

public partial class BudgetCreatePopUp : Popup<BudgetCreateDTO?>
{
    readonly IAccountRepository _accountsRepository;
    readonly ICategoryRepository _categoriesRepository;
    readonly IFamilyMemberRepository _familyMembersRepository;
    readonly IUserContext _user;

    List<Account> Accounts { get; set; } = new();
    List<Category> Categories { get; set; } = new();
    List<FamilyMember> FamilyMembers { get; set; } = new();

    List<FilterItem> SelectedFiltersDisplay = new();
    List<FilterItem> SelectedFiltersReturned = new();

    public BudgetCreatePopUp(
        IAccountRepository accounts,
        ICategoryRepository categories,
        IFamilyMemberRepository families,
        IUserContext user)
    {
        InitializeComponent();

        _accountsRepository = accounts;
        _categoriesRepository = categories;
        _familyMembersRepository = families;
        _user = user;

        Loaded += OnLoad;
    }

    private async void OnLoad(object sender, EventArgs e)
    {
        Clear();

        TypePicker.ItemsSource = Enum.GetValues(typeof(TransactionType));
        TypePicker.SelectedItem = TransactionType.Expense;


        Accounts = (await _accountsRepository.GetAll(_user.UserId)).ToList();
        Categories = (await _categoriesRepository.GetAll(_user.UserId)).ToList();
        FamilyMembers = (await _familyMembersRepository.GetAll(_user.UserId)).ToList();

        FilterTypePicker.ItemsSource = Enum.GetValues(typeof(BudgetFilterType));
        FilterTypePicker.SelectedItem = BudgetFilterType.Account;

        FilterValuePicker.ItemsSource = Accounts;
        FilterValuePicker.ItemDisplayBinding = new Binding("Name");
    }

    private async void OnFilterTypeChanged(object sender, EventArgs e)
    {
        var type = FilterTypePicker.SelectedItem as BudgetFilterType?;

        if (type is null)
        {
            return;
        }

        switch (type.Value)
        {
            case BudgetFilterType.Account:
                {
                    FilterValuePicker.SelectedItem = null;
                    FilterValuePicker.ItemsSource = Accounts;
                    
                    break;
                }
            case BudgetFilterType.Category:
                {
                    FilterValuePicker.SelectedItem = null;
                    FilterValuePicker.ItemsSource = Categories;
                    break;
                }
            case BudgetFilterType.FamilyMember:
                {
                    FilterValuePicker.SelectedItem = null;
                    FilterValuePicker.ItemsSource = FamilyMembers;
                    break;
                }
        }
    }

    private async void OnAddFilter(object sender, EventArgs e)
    {
        var type = (BudgetFilterType)FilterTypePicker.SelectedItem;

        var item = new FilterItem { Type = type };
        var itemDisplay = new FilterItem { Type = type };

        switch (type)
        {
            case BudgetFilterType.Account:
                {
                    var account = FilterValuePicker.SelectedItem as Account;

                    if (account is not null)
                    {
                        item.SelectedValue = account.Id.ToString();
                        itemDisplay.SelectedValue = account.Name;
                    }

                        break;
                }

            case BudgetFilterType.Category:
                {
                    var category = FilterValuePicker.SelectedItem as Category;

                    if (category is not null)
                    {
                        item.SelectedValue = category.Id.ToString();
                        itemDisplay.SelectedValue = category.Name;
                    }

                    break;
                }


            case BudgetFilterType.FamilyMember:
                {
                    var familyMember = FilterValuePicker.SelectedItem as FamilyMember;

                    if (familyMember is not null)
                    {
                        item.SelectedValue = familyMember.Id.ToString();
                        itemDisplay.SelectedValue = familyMember.Name;
                    }

                    break;
                }

        }

        SelectedFiltersReturned.Add(item);
        SelectedFiltersDisplay.Add(itemDisplay);

        FiltersView.ItemsSource = null;
        FiltersView.ItemsSource = SelectedFiltersDisplay;
    }

    private void OnRemoveFilter(object sender, EventArgs e)
    {
        //var button = sender as Button;
        //var item = button?.BindingContext as FilterItem;

        //if (item != null)
        //{
        //    Filters.Remove(item);

        //    FiltersView.ItemsSource = null;
        //    FiltersView.ItemsSource = Filters;
        //}
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        try
        {
            var name = NameEntry.Text;

            var limit = decimal.Parse(LimitEntry.Text);

            var startDate = StartDatePicker.Date;

            var endDate = EndDatePicker.Date;

            var filters = new List<(BudgetFilterType Type, string Value)>();

            filters.Add((BudgetFilterType.TransactionType, (TypePicker.SelectedItem as TransactionType?)?.ToString() ?? "Income"));

            foreach (var filterItem in SelectedFiltersReturned)
            {
                filters.Add((filterItem.Type, filterItem.SelectedValue ?? ""));
            }


            await CloseAsync(new BudgetCreateDTO(name,
                limit,
                startDate,
                endDate,
                filters));
        }
        catch (FormatException ex)
        {
            await Application.Current.MainPage.DisplayAlert("Can't Create Budget", $"{ex.Message}", "OK");
        }
        catch (ArgumentException ex)
        {
            await Application.Current.MainPage.DisplayAlert("Can't Create Budget", $"{ex.Message}", "OK");
        }

    }

    private void Clear()
    {
        NameEntry.Text = string.Empty;
        LimitEntry.Text = string.Empty;

        StartDatePicker.Date = DateTime.Now;
        EndDatePicker.Date = DateTime.Now.AddMonths(1);

        SelectedFiltersDisplay.Clear();
        SelectedFiltersReturned.Clear();

        FiltersView.ItemsSource = null;
        FiltersView.ItemsSource = SelectedFiltersDisplay;
    }
}