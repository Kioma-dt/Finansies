using BuisnessLogic.Entities;

namespace UI.Messages
{
    public class SelectedAccountChangedMessage
    {
        public Guid? SelectedAccountId { get; }

        public SelectedAccountChangedMessage(Guid? selectedAccountId)
        {
            SelectedAccountId = selectedAccountId;
        }

    }
}
