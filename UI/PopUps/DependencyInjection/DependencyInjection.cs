using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Popups;
using UI.PopUps.ViewModels;
using UI.PopUps.Abstraction;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.CategoryUseCasses.Commands;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Commands;
using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.BudgetUseCasses.Commands;
using BuisnessLogic.UseCases.TransfersUseCasses.Commands;
using UI.PopUps.Service;

namespace UI.PopUps.DependencyInjection
{
    public static class DependencyInjectionPopUps
    {
        public static IServiceCollection AddPopUps(this
            IServiceCollection services)
        {
            services.AddTransient<AccountCreatePopUp>();
            services.AddTransient<CategoryCreatePopUp>();
            services.AddTransient<FamilyMemberCreatePopUp>();
            services.AddTransient<TransactionCreatePopUp>();
            services.AddTransient<PlannedTransactionCreatePopUp>();
            services.AddTransient<BudgetCreatePopUp>();
            services.AddTransient<DebtCreatePopUp>();
            services.AddTransient<TransferCreatePopUp>();

            services.AddTransient<DateRangePopUp>();

            services.AddTransient<AccountCreatePopUpModel>();
            services.AddTransient<BudgetCreatePopUpModel>();
            services.AddTransient<CategoryCreatePopUpModel>();
            services.AddTransient<DateRangePopUpModel>();
            services.AddTransient<DebtCreatePopUpModel>();
            services.AddTransient<FamilyMemberCreatePopUpModel>();
            services.AddTransient<TransferCreatePopUpModel>();

            services.AddSingleton<IPopUpService, PopUpService>();

            return services;
        }
    }
}
