using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BuisnessLogic.Entities;
using BuisnessLogic.Enums;

namespace UI.Statistics
{
    public enum GraphSumType { Income, Expense, TotalSum, TotalIncrease};
    public class ChartData(ISeries[] Series,
        Axis[] XAxes)
    {
        public ISeries[] Series { get; set; } = Series;

        public Axis[] XAxes { get; set; } = XAxes;
    }

    public interface IAnalyticsService
    {
        ChartData BuildTransactionsDynamicChart(IEnumerable<Transaction> transactions,
            DateTime from, 
            DateTime to);

        ChartData BuildTransactionsColumnChart(IEnumerable<Transaction> transactions,
            DateTime from, 
            DateTime to, 
            Func<Transaction, string> groupSelector,
            GraphSumType graphSumType);

        ChartData BuildTransactionsPieChart(IEnumerable<Transaction> transactions,
            DateTime from,
            DateTime to,
            Func<Transaction, string> groupSelector,
            GraphSumType graphSumType);
    }

    public class AnalyticsService
        : IAnalyticsService
    {
        public ChartData BuildTransactionsPieChart(IEnumerable<Transaction> transactions,
            DateTime from, 
            DateTime to, 
            Func<Transaction, string> groupSelector,
            GraphSumType graphSumType)
        {
            transactions = transactions.Where(x => x.Date.Date >= from && x.Date.Date <= to);

            var groups = graphSumType switch 
            {
                GraphSumType.Income => transactions
                .Where(x => x.Type == TransactionType.Income)
                .GroupBy(groupSelector)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Total),

                GraphSumType.Expense => transactions
                .Where(x => x.Type == TransactionType.Expense)
                .GroupBy(groupSelector)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Total),

                GraphSumType.TotalSum => transactions
                .GroupBy(groupSelector)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Total),


                _ => transactions
                .GroupBy(groupSelector)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Sum(x => x.SignedAmount)
                })
                .OrderByDescending(x => x.Total),
            };

            var series = groups
                .Select(x => new PieSeries<decimal>
                {
                    Name = x.Name,
                    Values = new decimal[] { x.Total }
                })
                .ToArray();

            return new ChartData(series, []);
        }
        public ChartData BuildTransactionsColumnChart(IEnumerable<Transaction> transactions,
            DateTime from, 
            DateTime to, 
            Func<Transaction, string> groupSelector,
            GraphSumType graphSumType)
        {
            transactions = transactions.Where(x => x.Date.Date >= from && x.Date.Date <= to);

            var groups = graphSumType switch
            {
                GraphSumType.Income => transactions
                .Where(x => x.Type == TransactionType.Income)
                .GroupBy(groupSelector)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Total),

                GraphSumType.Expense => transactions
                .Where(x => x.Type == TransactionType.Expense)
                .GroupBy(groupSelector)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Total),

                GraphSumType.TotalSum => transactions
                .GroupBy(groupSelector)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Total),


                _ => transactions
                .GroupBy(groupSelector)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Sum(x => x.SignedAmount)
                })
                .OrderByDescending(x => x.Total),
            };

            var values = groups.Select(x => x.Total).ToList();
            var labels = groups.Select(x => x.Name).ToList();

            return new ChartData
                (
                    [
                        new ColumnSeries<decimal>()
                        {
                            Name = "Sum",
                            Values = values,
                        }
                    ],
                    [
                       new Axis()
                       {
                           Labels = labels
                       }
                    ]

                );
        }

        public ChartData BuildTransactionsDynamicChart(IEnumerable<Transaction> transactions,
            DateTime from, 
            DateTime to)
        {
            transactions = transactions
                .Where(x => x.Date.Date >= from && x.Date.Date <= to);

            var grouped = transactions
                .GroupBy(x => x.Date.Date)
                .OrderBy(g => g.Key);

            var income = new List<decimal>();
            var expense = new List<decimal>();
            var labels = new List<string>();

            foreach (var day in grouped)
            {
                income.Add
                (
                    day
                    .Where(x => x.Type == TransactionType.Income)
                    .Sum(x => x.Amount)
                );

                expense.Add
                (
                    day
                    .Where(x => x.Type == TransactionType.Expense)
                    .Sum(x => x.Amount)
                );

                labels.Add(day.Key.ToString("dd/MM"));
            }

            return new ChartData
                (
                     [
                        new LineSeries<decimal>
                        {
                            Name = "Income",
                            Values = income
                        },

                        new LineSeries<decimal>
                        {
                            Name = "Expense",
                            Values = expense
                        }
                    ],

                    [
                        new Axis
                        {
                            Labels = labels
                        }
                    ]

                );
        }

    }
}
