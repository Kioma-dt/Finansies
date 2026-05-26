using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.ViewModels;

namespace UI.OrderingServices
{
    public interface IPlannedTransactionsOrderingService
    {
        PlannedTransactionsOrderBy OrderBy { get; }
        IEnumerable<DisplayedPlanTransaction> Order(IEnumerable<DisplayedPlanTransaction> transactions, bool ascending);
    }

    public interface IPlannedTransactionsOrderingServiceFactory
    {
        IPlannedTransactionsOrderingService Create(PlannedTransactionsOrderBy orderBy);
    }
    public class PlannedTransactionsOrderingServiceFactory
        : IPlannedTransactionsOrderingServiceFactory
    {
        private readonly Dictionary<PlannedTransactionsOrderBy, IPlannedTransactionsOrderingService> _services;

        public PlannedTransactionsOrderingServiceFactory(IEnumerable<IPlannedTransactionsOrderingService> services)
        {
            _services = services.ToDictionary(s => s.OrderBy);
        }

        public IPlannedTransactionsOrderingService Create(PlannedTransactionsOrderBy orderBy)
        {
            var service = _services.GetValueOrDefault(orderBy);

            if (service is null)
            {
                throw new ArgumentException($"No ordering service found for {orderBy}", nameof(orderBy));
            }

            return service;
        }
    }

    public class PlannedTransactionsOrderingServiceByDescription 
        : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.Description;
        public IEnumerable<DisplayedPlanTransaction> Order(IEnumerable<DisplayedPlanTransaction> transactions, bool ascending)
        {
            return ascending
                ? transactions.OrderBy(t => t.Description)
                : transactions.OrderByDescending(t => t.Description);
        }
    }

    public class PlannedTransactionsOrderingServiceByStatus
        : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.Status;
        public IEnumerable<DisplayedPlanTransaction> Order(IEnumerable<DisplayedPlanTransaction> transactions, bool ascending)
        {
            return ascending
                ? transactions.OrderBy(t => t.Status)
                : transactions.OrderByDescending(t => t.Status);
        }
    }

    public class PlannedTransactionsOrderingServiceByDate : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.PlannedDate;
        public IEnumerable<DisplayedPlanTransaction> Order(
        IEnumerable<DisplayedPlanTransaction> transactions,
        bool ascending)
        {
            return ascending
                ? transactions.OrderBy(ParseDate)
                : transactions.OrderByDescending(ParseDate);
        }

        private DateTime ParseDate(DisplayedPlanTransaction transaction)
        {
            return DateTime.TryParseExact(
                transaction.PlannedDate,
                "dd.MM.yyyy",
                null,
                System.Globalization.DateTimeStyles.None,
                out var result)
                ? result
                : DateTime.MinValue;
        }
    }

    public class PlannedTransactionsOrderingServiceByAmount : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.Amount;
        public IEnumerable<DisplayedPlanTransaction> Order(
            IEnumerable<DisplayedPlanTransaction> transactions,
            bool ascending)
        {
            return ascending
                ? transactions.OrderBy(ParseAmount)
                : transactions.OrderByDescending(ParseAmount);
        }

        private decimal ParseAmount(DisplayedPlanTransaction transaction)
        {
            return decimal.TryParse(
                transaction.Amount,
                out var result)
                ? result
                : decimal.MinValue;
        }
    }

    public class PlannedTransactionsOrderingServiceByType : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.Type;
        public IEnumerable<DisplayedPlanTransaction> Order(IEnumerable<DisplayedPlanTransaction> transactions, bool ascending)
        {
            return ascending
                ? transactions.OrderBy(t => t.Type)
                : transactions.OrderByDescending(t => t.Type);
        }
    }

    public class PlannedTransactionsOrderingServiceByAccountName : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.AccountName;
        public IEnumerable<DisplayedPlanTransaction> Order(IEnumerable<DisplayedPlanTransaction> transactions, bool ascending)
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

    public class PlannedTransactionsOrderingServiceByCategoryName : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.CategoryName;
        public IEnumerable<DisplayedPlanTransaction> Order(
            IEnumerable<DisplayedPlanTransaction> transactions,
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

    public class PlannedTransactionsOrderingServiceByFamilyMemberName : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.FamilyMemberName;
        public IEnumerable<DisplayedPlanTransaction> Order(IEnumerable<DisplayedPlanTransaction> transactions, bool ascending)
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

    public class PlannedTransactionsOrderingServiceByDebtName : IPlannedTransactionsOrderingService
    {
        public PlannedTransactionsOrderBy OrderBy => PlannedTransactionsOrderBy.DebtName;
        public IEnumerable<DisplayedPlanTransaction> Order(IEnumerable<DisplayedPlanTransaction> transactions, bool ascending)
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
