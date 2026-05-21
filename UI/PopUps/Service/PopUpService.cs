using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

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
    }
}
