using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UI.PopUps.Abstraction;

namespace UI.PopUps.Service
{
    public interface IPopUpService
    {
        Task<TResult?> ShowPopUp<TResult, TPopUp>()
            where TPopUp : Popup<TResult>;

        Task<TResult?> ShowPopUp<TResult, TPopUp, TViewModel, TArgs>(
            TArgs args)
            where TPopUp : Popup<TResult>
            where TViewModel : class, IPopUpInitializable<TArgs>;
    }
}
