using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using UI.PopUps.Abstraction;

namespace UI.PopUps.Service
{
    public class PopUpService
        : IPopUpService
    {
        readonly IServiceProvider _service;

        public PopUpService(IServiceProvider service)
        {
            _service = service;
        }

        public async Task<TResult?> ShowPopUp<TResult, TPopUp>() 
            where TPopUp : Popup<TResult>
        {
            var popup = _service.GetRequiredService<TPopUp>();

            var result = await Application.Current.MainPage
                .ShowPopupAsync<TResult>(popup);

            return result.Result;
        }

        public async Task<TResult?> ShowPopUp<TResult, TPopUp, TViewModel, TArgs>(
            TArgs args)
            where TPopUp : Popup<TResult>
            where TViewModel : class, IPopUpInitializable<TArgs>
        {
            var vm = _service.GetRequiredService<TViewModel>();

            await vm.Initialize(args);

            var popup = _service.GetRequiredService<TPopUp>();

            popup.BindingContext = vm;

            var result = await Application.Current.MainPage
                .ShowPopupAsync<TResult>(popup);

            return result.Result;
        }
    }
}
