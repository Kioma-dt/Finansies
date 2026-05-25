using BuisnessLogic.UseCases.UserUseCasses.Commands;
using BuisnessLogic.UseCases.UserUseCasses.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Views;

namespace UI.ViewModels
{
    public partial class AuthentificationPageViewModel
        : ObservableObject
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;

        public AuthentificationPageViewModel(IMediator mediator, IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Name { get; set; } = String.Empty;

        [ObservableProperty]
        public partial string? Password { get; set; } = String.Empty;


        [RelayCommand]
        public async Task Login()
        {
            try
            {
                var userName = Name;
                
                if (String.IsNullOrWhiteSpace(userName))
                {
                    throw new ArgumentException("Enter User Name!");
                }

                var password = Password;

                if (String.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("Enter Password!");
                }

                if (password.Length < 6 || password.Length > 64)
                {
                    throw new ArgumentException("Invalid Password Lebgth! Should be >=6 and <=64");
                }

                var user = await _mediator.Send(new LogInQuery(userName, password));

                if (user == null)
                {
                    throw new ArgumentException("Can't Find Such User");
                }

                _userContext.SetUserId(user.Id);

                await Shell.Current.GoToAsync(nameof(MainPage));
            }
            catch (ArgumentException ex)
            {
                await Shell.Current.DisplayAlert(
                       "Error",
                       ex.Message,
                       "OK");
            }
        }

        [RelayCommand]
        public async Task Register()
        {
            try
            {
                var userName = Name;

                if (String.IsNullOrWhiteSpace(userName))
                {
                    throw new ArgumentException("Enter User Name!");
                }

                var password = Password;

                if (String.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("Enter Password!");
                }

                if (password.Length < 6 || password.Length > 64)
                {
                    throw new ArgumentException("Invalid Password Lebgth! Should be >=6 and <=64");
                }

                var user = await _mediator.Send(new RegisterCommand(userName, password));

                if (user == null)
                {
                    throw new ArgumentException("Can't Create Such User");
                }

                _userContext.SetUserId(user.Id);

                await Shell.Current.GoToAsync(nameof(MainPage));
            }
            catch (ArgumentException ex)
            {
                await Shell.Current.DisplayAlert(
                       "Error",
                       ex.Message,
                       "OK");
            }
        }

    }
}
