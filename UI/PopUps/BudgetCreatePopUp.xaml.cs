using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;

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
    public List<string> Values { get; set; } = new();
    public string? SelectedValue { get; set; }
}

public partial class BudgetCreatePopUp : Popup<BudgetCreateDTO?>
{
    readonly IAccountRepository _accounts;
    readonly ICategoryRepository _categories;
    readonly IFamilyMemberRepository _families;
    readonly IUserContext _user;

    List<FilterItem> Filters = new();

    public BudgetCreatePopUp(
        IAccountRepository accounts,
        ICategoryRepository categories,
        IFamilyMemberRepository families,
        IUserContext user)
    {
        InitializeComponent();

        _accounts = accounts;
        _categories = categories;
        _families = families;
        _user = user;

        Loaded += OnLoad;
    }

    private async void OnLoad(object sender, EventArgs e)
    {
        Clear();

        TypePicker.ItemsSource = Enum.GetValues(typeof(TransactionType));
        TypePicker.SelectedItem = TransactionType.Expense;

        FilterTypePicker.ItemsSource = Enum.GetValues(typeof(BudgetFilterType));
        FiltersView.ItemsSource = Filters;
    }

    private async void OnAddFilter(object sender, EventArgs e)
    {
        var type = (BudgetFilterType)FilterTypePicker.SelectedItem;

        var item = new FilterItem { Type = type };

        var userId = _user.UserId;

        switch (type)
        {
            case BudgetFilterType.Account:
                item.Values = (await _accounts.GetAllScalar(userId)).Select();
                break;

            case BudgetFilterType.Category:
                item.Values = (await _categories.GetAllScalar(userId))!.Cast<object>().ToList();
                break;

            case BudgetFilterType.FamilyMember:
                item.Values = (await _families.GetAllScalar(userId))!.Cast<object>().ToList();
                break;

            case BudgetFilterType.TransactionType:
                item.Values = Enum.GetValues(typeof(TransactionType)).Cast<object>().ToList();
                break;
        }

        Filters.Add(item);

        // обновляем вручную
        FiltersView.ItemsSource = null;
        FiltersView.ItemsSource = Filters;
    }

    private void OnRemoveFilter(object sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button?.BindingContext as FilterItem;

        if (item != null)
        {
            Filters.Remove(item);

            FiltersView.ItemsSource = null;
            FiltersView.ItemsSource = Filters;
        }
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        var name = NameEntry.Text;

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartDate = StartDatePicker.Date,
            EndDate = EndDatePicker.Date,
            UserId = _user.UserId
        };

        // тут потом соберешь Filters в BudgetFilter

        await CloseAsync(budget);
    }

    private void Clear()
    {
        NameEntry.Text = string.Empty;

        StartDatePicker.Date = DateTime.Now;
        EndDatePicker.Date = DateTime.Now.AddMonths(1);

        Filters.Clear();
        FiltersView.ItemsSource = null;
        FiltersView.ItemsSource = Filters;
    }
}