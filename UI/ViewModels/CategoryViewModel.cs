using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Popups;
using UI.Messages;

namespace UI.ViewModels
{
    public class CategoryNode
    {
        public Category Category { get; set; } = null!;
        public int Level { get; set; }
    }

    public partial class CategoryViewModel : ObservableObject, IRecipient<DataBaseChangedMessage>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserContext _user;
        private readonly CategoryCreatePopUp _categoryCreatePopUp;

        private List<Category> _categories = new();


        public ObservableCollection<CategoryNode> FlatCategories { get; } = new();

        [ObservableProperty]
        public partial bool IsLoaded { get; set; } = false;

        public CategoryViewModel(
            ICategoryRepository categoryRepository,
            IUserContext user,
            CategoryCreatePopUp categoryCreatePopUp)
        {
            _categoryRepository = categoryRepository;
            _user = user;
            _categoryCreatePopUp = categoryCreatePopUp;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
        }

        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Categories)
            {
                _categories = await _categoryRepository.GetAll(_user.UserId) ?? new();

                BuildTree();
            }
        }


        //[RelayCommand]
        //public async Task Load()
        //{
        //    _categories = await _categoryRepository.GetAll(_user.UserId) ?? new();

        //    BuildTree();
        //}

        [RelayCommand]
        public async Task AddCategory()
        {
            var result = await Application.Current.MainPage
                .ShowPopupAsync<Category?>(_categoryCreatePopUp);

            var category = result.Result;

            if (category is null)
                return;

            category.UserId = _user.UserId;

            await _categoryRepository.Add(category);

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Categories));
        }


        private void BuildTree()
        {
            FlatCategories.Clear();

            void Add(Category cat, int level)
            {
                FlatCategories.Add(new CategoryNode
                {
                    Category = cat,
                    Level = level
                });

                foreach (var child in cat.Children)
                    Add(child, level + 1);
            }

            var roots = _categories.Where(x => x.ParentId == null);

            foreach (var root in roots)
                Add(root, 0);
        }
    }
}
