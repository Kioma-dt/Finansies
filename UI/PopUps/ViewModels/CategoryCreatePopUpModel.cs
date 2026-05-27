using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using BuisnessLogic.UseCases.CategoryUseCasses.Commands;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace UI.PopUps.ViewModels
{
    public partial class CategoryCreatePopUpModel
        : ObservableObject
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;

        public Action<CreateCategoryCommand?>? CloseAction { get; set; }

        public CategoryCreatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Name { get; set; }

        [ObservableProperty]
        public partial string? Description { get; set; }

        [ObservableProperty]
        public partial Category? SelectedParent { get; set; }

        public ObservableCollection<Category> Parents { get; set; } = new();

        public async Task Initialize()
        {
            Parents.Clear();

            Parents.Insert(0, new Category() { Id = Guid.Empty, Name = "-No Parent-" });

            var parents = await _mediator.Send(new GetAllCategoriesQuery(_userContext.UserId));

            foreach (var parent in parents)
            {
                Parents.Add(parent);
            }

            SelectedParent = Parents.FirstOrDefault();

            Name = string.Empty;
            Description = string.Empty;
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

                var description = Description;

                if (String.IsNullOrWhiteSpace(description))
                {
                    throw new ArgumentException($"Enter Description!");
                }

                var parent = SelectedParent;


                CloseAction?.Invoke(new CreateCategoryCommand(_userContext.UserId,
                    name,
                    description,
                    parent == null || parent.Id == Guid.Empty ? null : parent.Id));
            }
            catch (ArgumentException ex)
            {
                await Shell.Current.DisplayAlert(
                       "Can't Create Category",
                       ex.Message,
                       "OK");
            }
           
        }
    }
}
