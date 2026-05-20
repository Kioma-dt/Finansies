using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.CategoryUseCasses.Commands;
using CommunityToolkit.Maui.Views;

using UI.PopUps.ViewModels;

namespace UI.Popups;

public partial class CategoryCreatePopUp : Popup<CreateCategoryCommand?>
{
    public CategoryCreatePopUp(CategoryCreatePopUpModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        viewModel.CloseAction = async result =>
        {
            await CloseAsync(result);
        };

        Loaded += async (_, _) =>
        {
            await viewModel.Initialize();
        };
    }
    //}
    //private async void OnLoad(object sender, EventArgs e)
    //{
    //    this.Clear();

    //    Categories = (await _categoryRepository.GetAll(UserId)).ToList();

    //    Categories.Insert(0, new Category() { Id = Guid.Empty, Name = "-No Parent-" });
    //    ParentPicker.ItemsSource = Categories;
    //    ParentPicker.ItemDisplayBinding = new Binding("Name");
    //    ParentPicker.SelectedIndex = 0;
    //}
    //private async void OnCancel(object sender, EventArgs e)
    //{
    //    await CloseAsync(null);
    //}

    //private async void OnCreate(object sender, EventArgs e)
    //{
    //    var name = NameEntry.Text;
    //    var desciption = DescriptionEntry.Text;

    //    var parent = ParentPicker.SelectedItem as Category;


    //    await CloseAsync(new Category
    //    {
    //        Name = name,
    //        Description = desciption,
    //        ParentId = parent == null || parent.Id == Guid.Empty ? null : parent.Id
    //    });
    //}

    //public void Clear()
    //{
    //    NameEntry.Text = string.Empty;
    //    DescriptionEntry.Text = string.Empty;

    //    if (ParentPicker.ItemsSource != null)
    //        ParentPicker.SelectedIndex = 0;
    //}
}