using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;

namespace UI.Popups;

public partial class DebtCreatePopUp : Popup<DebtCreateDTO?>
{
    readonly ICategoryRepository _categoryRepo;
    readonly IFamilyMemberRepository _familyRepo;
    readonly IUserContext _user;

    List<Category> Categories = new();
    List<FamilyMember> Families = new();

    public DebtCreatePopUp(
        ICategoryRepository categoryRepo,
        IFamilyMemberRepository familyRepo,
        IUserContext user)
    {
        InitializeComponent();

        _categoryRepo = categoryRepo;
        _familyRepo = familyRepo;
        _user = user;

        Loaded += OnLoad;
    }

    private async void OnLoad(object sender, EventArgs e)
    {
        Clear();

        Categories = (await _categoryRepo.GetAll(_user.UserId)).ToList();
        Families = await _familyRepo.GetAllScalar(_user.UserId) ?? new();

        Categories.Insert(0, new Category { Id = Guid.Empty, Name = "-No Category-" });
        Families.Insert(0, new FamilyMember { Id = Guid.Empty, Name = "-No Member-" });

        CategoryPicker.ItemsSource = Categories;
        CategoryPicker.ItemDisplayBinding = new Binding("Name");
        CategoryPicker.SelectedIndex = 0;

        FamilyPicker.ItemsSource = Families;
        FamilyPicker.ItemDisplayBinding = new Binding("Name");
        FamilyPicker.SelectedIndex = 0;

        DebtTypePicker.ItemsSource = Enum.GetValues(typeof(DebtType));
        DebtTypePicker.SelectedIndex = 0;

        InterestTypePicker.ItemsSource = Enum.GetValues(typeof(InterestType));
        InterestTypePicker.SelectedItem = InterestType.None;

        CapitalisationPicker.ItemsSource = new List<string> { "Yearly", "Monthly" };
        CapitalisationPicker.SelectedIndex = 1;

        AutoCalculateCheckBox.IsChecked = false;

        PaymentPeriodicityPicker.ItemsSource = Enum.GetValues(typeof(TransactionPeriodicity));
        PaymentPeriodicityPicker.SelectedItem = TransactionPeriodicity.Monthly;
        PaymentPeriodicityPicker.IsEnabled = false;

        UpdateInterestField();
    }

    private void OnInterestTypeChanged(object sender, EventArgs e)
    {
        UpdateInterestField();
    }

    private void UpdateInterestField()
    {
        var type = (InterestType)InterestTypePicker.SelectedItem;

        switch (type)
        {
            case InterestType.None:
                InterestValueEntry.IsEnabled = false;
                InterestValueEntry.Text = "0";
                InterestValueEntry.Placeholder = "No interest";
                break;

            case InterestType.Simple:
            case InterestType.Complex:
                InterestValueEntry.IsEnabled = true;
                InterestValueEntry.Placeholder = "Interest %";
                break;

            case InterestType.Fixed:
                InterestValueEntry.IsEnabled = true;
                InterestValueEntry.Placeholder = "Fixed amount";
                break;
        }
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        try
        {
            var amount = decimal.Parse(AmountEntry.Text);
            var interestValue = decimal.Parse(InterestValueEntry.Text);

            var capitalisations = CapitalisationPicker.SelectedIndex == 0 ? 1 : 12;

            var category = CategoryPicker.SelectedItem as Category;
            var family = FamilyPicker.SelectedItem as FamilyMember;

            var autoCalculate = AutoCalculateCheckBox.IsChecked;

            TransactionPeriodicity? payments = null;

            if (autoCalculate)
            {
                payments = (TransactionPeriodicity)PaymentPeriodicityPicker.SelectedItem;
            }

            var debt = new DebtCreateDTO
            (
                Name: NameEntry.Text ?? "",
                Amount: amount,
                Type: (DebtType)DebtTypePicker.SelectedItem,
                StartDate: StartDatePicker.Date,
                EndDate: EndDatePicker.Date,

                CategoryId: category?.Id == Guid.Empty ? null : category?.Id,
                FamilyMemberId: family?.Id == Guid.Empty ? null : family?.Id,
                CapitalisatonsPerYear: capitalisations,

                InterestType: (InterestType)InterestTypePicker.SelectedItem,

                InterestRate: interestValue,
                FixedAddition: interestValue,
                IsAutoPlanned: autoCalculate,
                TransactionPeriodicity: payments ?? TransactionPeriodicity.Once
            );

            await CloseAsync(debt);
        }
        catch (FormatException ex)
        {
            await Application.Current.MainPage
                .DisplayAlert("Can't Create Debt", ex.Message, "OK");
        }
    }

    private void OnAutoCalculateChanged(object sender, CheckedChangedEventArgs e)
    {
        PaymentPeriodicityPicker.IsEnabled = e.Value;

        //if (!e.Value)
        //{
        //    PaymentsPerYearEntry.Text = "";
        //}
    }

    private void Clear()
    {
        NameEntry.Text = "";
        AmountEntry.Text = "";

        StartDatePicker.Date = DateTime.Now;
        EndDatePicker.Date = DateTime.Now.AddMonths(1);

        InterestValueEntry.Text = "0";
    }
}