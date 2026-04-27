using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;

namespace UI.ViewModels
{
    public class CategoryNode
    {
        public Category Category { get; set; } = null!;
        public int Level { get; set; }
    }

    public partial class CategoryViewModel : ObservableObject
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserContext _user;

        private List<Category> _categories = new();

        public ObservableCollection<CategoryNode> FlatCategories { get; } = new();

        public CategoryViewModel(
            ICategoryRepository categoryRepository,
            IUserContext user)
        {
            _categoryRepository = categoryRepository;
            _user = user;
        }

        [RelayCommand]
        public async Task Load()
        {
            _categories = await _categoryRepository.GetAll(_user.UserId) ?? new();

            BuildTree();
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
