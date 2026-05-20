using BuisnessLogic.Entities;
using BuisnessLogic.DTO;
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
    public class FamilyMemberViewItem
    {
        public FamilyMember? FamilyMember { get; set; }

        public int TransactionsCount { get; set; }

        public decimal TotalAmount { get; set; }
    }
    public partial class FamilyMemberViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>
    {
        private readonly IFamilyMemberRepository _familyMemberRepository;
        private readonly IUserContext _user;
        private readonly FamilyMemberCreatePopUp _popup;

        private List<FamilyMember> _familyMembers = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        public ObservableCollection<FamilyMemberViewItem> FamilyMemberItems { get; } = new();


        public FamilyMemberViewModel(
            IFamilyMemberRepository familyMemberRepository,
            IUserContext user,
            FamilyMemberCreatePopUp popup)
        {
            _familyMemberRepository = familyMemberRepository;
            _user = user;
            _popup = popup;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
        }
        //.Include()
        //            .Include(
        //            .Include()
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init
                || message.Type == DataBaseChangedMessageType.FamilyMembers
                || message.Type == DataBaseChangedMessageType.Transactions)
            {
                _familyMembers= (await _familyMemberRepository.GetAll(_user.UserId,
                    x => x.Transactions,
                    x => x.PlannedTransactions,
                    x => x.Debts)).ToList();

                this.CreateItems();
            }
        }

        public void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            this.CreateItems();
        }


        [RelayCommand]
        public async Task AddFamilyMember()
        {
            var result = await Application.Current.MainPage
                .ShowPopupAsync<FamilyMemberCreateDTO?>(_popup);

            var familyMember = result.Result;

            if (familyMember is null)
                return;


            await _familyMemberRepository.Add(new FamilyMember()
            {
                Name = familyMember.Name,
                UserId = _user.UserId
            });

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.FamilyMembers));
        }

        private void CreateItems()
        {
            FamilyMemberItems.Clear();

            foreach (var fm in _familyMembers)
            {
                FamilyMemberItems.Add(new FamilyMemberViewItem()
                {
                    FamilyMember = fm,
                    TransactionsCount = fm.PeriodTransactionsNumber(_startDate, _endDate),
                    TotalAmount = fm.PeriodTransactionsSum(_startDate, _endDate)
                });
            }
        }
       
    }
}
