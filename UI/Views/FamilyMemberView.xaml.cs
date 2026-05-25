using UI.ViewModels;

namespace UI.Views;

public partial class FamilyMemberView : ContentView
{
    public FamilyMemberView(FamilyMemberViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.LoadCommand.Execute(null);
    }

    //public async Task LoadContent()
    //{
    //    if (BindingContext is CategoryViewModel vm && !vm.IsLoaded)
    //    {
    //        vm.IsLoaded = true;
    //        await vm.LoadCommand.ExecuteAsync(null);
    //    }
    //}
}