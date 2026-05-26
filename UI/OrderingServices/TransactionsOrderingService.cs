using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UI.ViewModels;

namespace UI.OrderingServices
{
    public interface ITransactionsOrderingService
    {
        TransactionsOrderBy OrderBy { get; }
        IEnumerable<DisplayedTransaction> Order(IEnumerable<DisplayedTransaction> transactions, bool ascending);
    }

    public interface ITransactionsOrderingServiceFactory
    {
        ITransactionsOrderingService Create(TransactionsOrderBy orderBy);
    }
    public class TransactionsOrderingServiceFactory
        : ITransactionsOrderingServiceFactory
    {
        private readonly Dictionary<TransactionsOrderBy, ITransactionsOrderingService> _services;

        public TransactionsOrderingServiceFactory(IEnumerable<ITransactionsOrderingService> services)
        {
            _services = services.ToDictionary(s => s.OrderBy);
        }

        public ITransactionsOrderingService Create(TransactionsOrderBy orderBy)
        {
            var service = _services.GetValueOrDefault(orderBy);

            if (service is null)
            {
                throw new ArgumentException($"No ordering service found for {orderBy}", nameof(orderBy));
            }

            return service;
        }
    }

    public class TransactionsOrderingServiceByDescription : ITransactionsOrderingService
    {
        public TransactionsOrderBy OrderBy => TransactionsOrderBy.Description;
        public IEnumerable<DisplayedTransaction> Order(IEnumerable<DisplayedTransaction> transactions, bool ascending)
        {
            return ascending
                ? transactions.OrderBy(t => t.Description)
                : transactions.OrderByDescending(t => t.Description);
        }
    }

    public class TransactionsOrderingServiceByDate : ITransactionsOrderingService
    {
        public TransactionsOrderBy OrderBy => TransactionsOrderBy.Date;
        public IEnumerable<DisplayedTransaction> Order(
        IEnumerable<DisplayedTransaction> transactions,
        bool ascending)
        {
            return ascending
                ? transactions.OrderBy(ParseDate)
                : transactions.OrderByDescending(ParseDate);
        }

        private DateTime ParseDate(DisplayedTransaction transaction)
        {
            return DateTime.TryParseExact(
                transaction.Date,
                "dd.MM.yyyy",
                null,
                System.Globalization.DateTimeStyles.None,
                out var result)
                ? result
                : DateTime.MinValue;
        }
    }

    public class TransactionsOrderingServiceByAmount : ITransactionsOrderingService
    {
        public TransactionsOrderBy OrderBy => TransactionsOrderBy.Amount;
        public IEnumerable<DisplayedTransaction> Order(
            IEnumerable<DisplayedTransaction> transactions,
            bool ascending)
        {
            return ascending
                ? transactions.OrderBy(ParseAmount)
                : transactions.OrderByDescending(ParseAmount);
        }

        private decimal ParseAmount(DisplayedTransaction transaction)
        {
            return decimal.TryParse(
                transaction.Amount,
                out var result)
                ? result
                : decimal.MinValue;
        }
    }

    public class TransactionsOrderingServiceByType : ITransactionsOrderingService
    {
        public TransactionsOrderBy OrderBy => TransactionsOrderBy.Type;
        public IEnumerable<DisplayedTransaction> Order(IEnumerable<DisplayedTransaction> transactions, bool ascending)
        {
            return ascending
                ? transactions.OrderBy(t => t.Type)
                : transactions.OrderByDescending(t => t.Type);
        }
    }

    public class TransactionsOrderingServiceByAccountName : ITransactionsOrderingService
    {
        public TransactionsOrderBy OrderBy => TransactionsOrderBy.AccountName;
        public IEnumerable<DisplayedTransaction> Order(IEnumerable<DisplayedTransaction> transactions, bool ascending)
        {
            return ascending
            ? transactions
                .OrderBy(t => t.AccountName is null)
                .ThenBy(t => t.AccountName)

            : transactions
                .OrderBy(t => t.AccountName is null)
                .ThenByDescending(t => t.AccountName);
        }
    }

    public class TransactionsOrderingServiceByCategoryName : ITransactionsOrderingService
    {
        public TransactionsOrderBy OrderBy => TransactionsOrderBy.CategoryName;
        public IEnumerable<DisplayedTransaction> Order(
            IEnumerable<DisplayedTransaction> transactions,
            bool ascending)
        {
            return ascending
                ? transactions
                    .OrderBy(t => t.CategoryName is null)
                    .ThenBy(t => t.CategoryName)

                : transactions
                    .OrderBy(t => t.CategoryName is null)
                    .ThenByDescending(t => t.CategoryName);
        }
    }

    public class TransactionsOrderingServiceByFamilyMemberName : ITransactionsOrderingService
    {
        public TransactionsOrderBy OrderBy => TransactionsOrderBy.FamilyMemberName;
        public IEnumerable<DisplayedTransaction> Order(IEnumerable<DisplayedTransaction> transactions, bool ascending)
        {
            return ascending
            ? transactions
                .OrderBy(t => t.FamilyMemberName is null)
                .ThenBy(t => t.FamilyMemberName)

            : transactions
                .OrderBy(t => t.FamilyMemberName is null)
                .ThenByDescending(t => t.FamilyMemberName);
        }
    }

    public class TransactionsOrderingServiceByDebtName : ITransactionsOrderingService
    {
        public TransactionsOrderBy OrderBy => TransactionsOrderBy.DebtName;
        public IEnumerable<DisplayedTransaction> Order(IEnumerable<DisplayedTransaction> transactions, bool ascending)
        {
            return ascending
            ? transactions
                .OrderBy(t => t.DebtName is null)
                .ThenBy(t => t.DebtName)

            : transactions
                .OrderBy(t => t.DebtName is null)
                .ThenByDescending(t => t.DebtName);
        }
    }
}
