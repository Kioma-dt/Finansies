using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;

namespace UI.Popups;

public partial class CategoryCreatePopUp : Popup<Category?>
{
    readonly ICategoryRepository _categoryRepository;
    List<Category> Categories { get; set; } = new();
    Guid UserId { get; set; }
    public CategoryCreatePopUp(ICategoryRepository categoryRepository, IUserContext user)
    {
        InitializeComponent();

        _categoryRepository = categoryRepository;

        UserId = user.UserId;

        Loaded += OnLoad;
    }
    private async void OnLoad(object sender, EventArgs e)
    {
        this.Clear();

        Categories = await _categoryRepository.GetAllScalar(UserId) ?? new();

        Categories.Insert(0, new Category() { Id = Guid.Empty, Name = "-No Parent-" });

        ParentPicker.ItemsSource = Categories;
        ParentPicker.ItemDisplayBinding = new Binding("Name");
    }
    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        var name = NameEntry.Text;
        var desciption = DescriptionEntry.Text;

        var parent = ParentPicker.SelectedItem as Category;


        await CloseAsync(new Category
        {
            Name = name,
            Description = desciption,
            ParentId = parent == null || parent.Id == Guid.Empty ? null : parent.Id,
            Parent = parent?.Id == Guid.Empty ? null : parent
        });
    }

    public void Clear()
    {
        NameEntry.Text = string.Empty;
        DescriptionEntry.Text = string.Empty;

        if (ParentPicker.ItemsSource != null)
            ParentPicker.SelectedIndex = 0;
    }
}