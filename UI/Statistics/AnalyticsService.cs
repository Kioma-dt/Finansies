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
    public class ChartData(ISeries[] Series,
        Axis[] XAxes)
    {
        public ISeries[] Series { get; set; } = Series;

        public Axis[] XAxes { get; set; } = XAxes;
    }

    public interface IAnalyticsService
    {
        ChartData BuildTransactionChart(IEnumerable<Transaction> transactions, DateTime from, DateTime to);
    }

    public class AnalyticsService
        : IAnalyticsService
    {
        public ChartData BuildTransactionChart(IEnumerable<Transaction> transactions,
            DateTime from, 
            DateTime to)
        {
            transactions = transactions
                .Where(x => x.Date >= from && x.Date <= to);

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
