using BuisnessLogic.Entities;

namespace BuisnessLogic.Tests;

using BuisnessLogic.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using UI.Statistics;
using Xunit;
using LiveChartsCore;

public class AnalyticsServiceTests
{
    private readonly AnalyticsService _service = new();

    private static List<Transaction> CreateTransactions()
    {
        return new List<Transaction>
        {
            new Transaction
            {
                Date = new DateTime(2024, 01, 01),
                Type = TransactionType.Income,
                Amount = 100
            },
            new Transaction
            {
                Date = new DateTime(2024, 01, 01),
                Type = TransactionType.Expense,
                Amount = 40
            },
            new Transaction
            {
                Date = new DateTime(2024, 01, 02),
                Type = TransactionType.Income,
                Amount = 200
            },
            new Transaction
            {
                Date = new DateTime(2024, 01, 02),
                Type = TransactionType.Expense,
                Amount = 50
            }
        };
    }

    #region Dynamic Chart

    [Fact]
    public void BuildDynamicChart_ShouldGroupByDate()
    {
        var data = CreateTransactions();

        var result = _service.BuildTransactionsDynamicChart(
            data,
            new DateTime(2024, 01, 01),
            new DateTime(2024, 01, 02));

        result.Series.Length.Should().Be(2); // Income + Expense
        result.XAxes.Length.Should().Be(1);
        result.XAxes[0].Labels.Should().ContainInOrder("01.01", "02.01");
    }

    #endregion

    #region Column Chart

    [Fact]
    public void BuildColumnChart_ShouldGroupByCategorySelector()
    {
        var data = CreateTransactions();

        var result = _service.BuildTransactionsColumnChart(
            data,
            new DateTime(2024, 01, 01),
            new DateTime(2024, 01, 02),
            x => x.Type.ToString(),
            GraphSumType.TotalSum);

        result.Series.Length.Should().Be(1);
        result.XAxes[0].Labels.Should().NotBeEmpty();
    }

    [Fact]
    public void BuildColumnChart_ShouldRespectDateFilter()
    {
        var data = CreateTransactions();

        var result = _service.BuildTransactionsColumnChart(
            data,
            new DateTime(2024, 01, 02),
            new DateTime(2024, 01, 02),
            x => x.Type.ToString(),
            GraphSumType.TotalSum);

        result.XAxes[0].Labels.Should().OnlyContain(x => x == "Income" || x == "Expense");
    }

    #endregion

    #region Pie Chart

    [Fact]
    public void BuildPieChart_ShouldFilterByDate()
    {
        var data = CreateTransactions();

        var result = _service.BuildTransactionsPieChart(
            data,
            new DateTime(2024, 01, 02),
            new DateTime(2024, 01, 02),
            x => x.Type.ToString(),
            GraphSumType.TotalSum);

        result.Series.Should().NotBeEmpty();
    }

    #endregion

    #region Edge cases

    [Fact]
    public void Should_Handle_Empty_Input()
    {
        var result = _service.BuildTransactionsDynamicChart(
            new List<Transaction>(),
            DateTime.Now,
            DateTime.Now);

        result.Series.Should().HaveCount(2);
        result.XAxes.Should().HaveCount(1);
        result.XAxes[0].Labels.Should().BeEmpty();
    }

    #endregion
}
