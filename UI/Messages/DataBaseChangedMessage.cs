using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Messages
{
    public enum DataBaseChangedMessageType { Init, 
        Accounts, 
        Budgets, 
        Categories, 
        Debts, 
        FamilyMembers,
        PlannedTransactions,
        Transactions, 
        TransactionTags, 
        Transfers,
        Users}
    public class DataBaseChangedMessage
    {
        public DataBaseChangedMessageType Type { get;}
        public DataBaseChangedMessage(DataBaseChangedMessageType type)
        {
            Type = type;
        }
    }
}
