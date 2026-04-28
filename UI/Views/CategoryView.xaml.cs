using UI.ViewModels;

namespace UI.Views;

public partial class CategoryView : ContentView
{
    public CategoryView(CategoryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    public async Task LoadContent()
    {
        if (BindingContext is CategoryViewModel vm && !vm.IsLoaded)
        {
            vm.IsLoaded = true;
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}