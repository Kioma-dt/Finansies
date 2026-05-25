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
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Commands;
using UI.PopUps.Service;

namespace UI.ViewModels
{
    public class DisplayedFamilyMember(Guid Id,
        string? Name,
        string? TransactionsCount,
        string? TotalAcmount)
    {
        public Guid Id { get; } = Id;

        public string? Name { get; } = Name;

        public string? TransactionsCount { get; } = TransactionsCount;

        public string? TotalAmount { get; } = TotalAcmount;
    }
    public partial class FamilyMemberViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>,
        IRecipient<SelectedAccountChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IPopUpService _popupService;

        private List<FamilyMember> _familyMembers = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        private Guid? _selectedAccountId = null;

        public ObservableCollection<DisplayedFamilyMember> DisplayedFamilyMembers { get; } = new();


        public FamilyMemberViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popup)
        {
            _mediator = mediator;
            _userContext = user;
            _popupService = popup;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<SelectedAccountChangedMessage>(this);
        }

        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init
                || message.Type == DataBaseChangedMessageType.FamilyMembers
                || message.Type == DataBaseChangedMessageType.Transactions)
            {
                _familyMembers.Clear();

                var familyMembers = await _mediator.Send(new GetAllFamilyMembersQuery(_userContext.UserId));

                foreach (var familyMember in familyMembers) 
                {
                    _familyMembers.Add(familyMember);
                }

                this.ShowFamilyMembers();
            }
        }

        public void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            this.ShowFamilyMembers();
        }
        public void Receive(SelectedAccountChangedMessage message)
        {
            _selectedAccountId = message.SelectedAccountId;

            this.ShowFamilyMembers();
        }

        [RelayCommand]
        public async Task AddFamilyMember()
        {
            var familyMember = await _popupService.ShowPopUp<CreateFamilyMemberCommand?, FamilyMemberCreatePopUp>();

            if (familyMember is null)
                return;


            await _mediator.Send(new CreateFamilyMemberCommand(_userContext.UserId, familyMember.Name));

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.FamilyMembers));
        }

        private void ShowFamilyMembers()
        {
            DisplayedFamilyMembers.Clear();

            foreach (var fm in _familyMembers)
            {
                DisplayedFamilyMembers.Add(new DisplayedFamilyMember(fm.Id, 
                    fm.Name, 
                    fm.PeriodTransactionsNumber(_startDate, _endDate, _selectedAccountId).ToString(),
                    fm.PeriodTransactionsSum(_startDate, _endDate, _selectedAccountId).ToString()));
            }
        }
    }
}
