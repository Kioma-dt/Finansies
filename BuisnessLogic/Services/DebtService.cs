using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands;

namespace BuisnessLogic.Services
{
    

    //public interface IDebtService
    //{
    //    Task<List<Debt>> GetAll(Guid userId);
    //    Task CreateDebt(Guid userId, DebtCreateDTO dto);
    //    Task PayOffDebt(Guid userId,
    //        Guid debtId,
    //        decimal amount,
    //        DateTime date);

    //    Task<List<Transaction>> GetRelevantTransactions(Guid userId, Guid debtId);
    //    Task UpdateDebt(Guid userId,
    //        Guid debtId,
    //        DateTime date);
    //}
    //public class DebtService : IDebtService
    //{
    //    readonly IMediator _mediator;
    //    readonly IDebtRepository _debtRepository;
    //    readonly IDebtInterestCalculatorProvider _debtInterestCalculatorProvider;

    //    public DebtService(IDebtRepository debtRepository, 
    //        IDebtInterestCalculatorProvider debtInterestCalculatorProvider,
    //        IMediator mediator)
    //    {
    //        _debtRepository = debtRepository;
    //        _mediator = mediator;
    //        _debtInterestCalculatorProvider = debtInterestCalculatorProvider;
    //    }

    //    public async Task CreateDebt(Guid userId, DebtCreateDTO dto)
    //    {
    //        var id = Guid.NewGuid();
    //        var debt = new Debt()
    //        {
    //            Id = id,
    //            Name = dto.Name,
    //            StartAmount = dto.Amount,
    //            TotalAmount = dto.Amount,
    //            PaidAmount = 0,
    //            InterestRate = dto.InterestRate,
    //            CapitalisationsPerYear = dto.CapitalisatonsPerYear,
    //            FixedAddition = dto.FixedAddition,
    //            Type = dto.Type,
    //            InterestType = dto.InterestType,
    //            StartDate = dto.StartDate,
    //            LastPaidDate = dto.StartDate,
    //            EndDate = dto.EndDate,
    //            CategoryId = dto.CategoryId,
    //            FamilyMemberId = dto.FamilyMemberId,
    //            UserId = userId
    //        };

    //        await _debtRepository.Add(debt);

    //        if (dto.IsAutoPlanned)
    //        {
    //            await _mediator.Send(new PlanDebtPaymentsCommand(userId, id, dto.TransactionPeriodicity));
    //        }
    //    }

    //    public async Task<List<Debt>> GetAll(Guid userId) => (await _debtRepository.GetAll(userId,
    //        x => x.FamilyMember,
    //        x => x.Category,
    //        x => x.PlannedTransactions,
    //        x => x.Transactions)).ToList();

    //    public async Task<List<Transaction>> GetRelevantTransactions(Guid userId, Guid debtId)
    //    {
    //        var debt = await _debtRepository.GetById(userId, 
    //            debtId,
    //            x => x.Transactions);

    //        if (debt is null)
    //        {
    //            throw new ArgumentException($"No Debt With Id: {debtId}");
    //        }

    //        return debt.Transactions;
    //    }

    //    public async Task PayOffDebt(Guid userId, Guid debtId, decimal amount, DateTime date)
    //    {
    //        var debt = await _debtRepository.GetById(userId, debtId);

    //        if (debt is null)
    //        {
    //            throw new ArgumentException($"No Debt with Id: {debtId}");
    //        }

    //        debt.MakeAPayment(amount, date);

    //        await _debtRepository.Update(debt);
    //    }

    //    public async Task UpdateDebt(Guid userId, Guid debtId, DateTime date)
    //    {
    //        var debt = await _debtRepository.GetById(userId, debtId);

    //        if (debt is null)
    //        {
    //            throw new ArgumentException($"No Debt with Id: {debtId}");
    //        }

    //        //if (date > debt.EndDate)
    //        //{
    //        //    throw new Exception("The Debt Is Overdue!");
    //        //}

    //        var calculator = _debtInterestCalculatorProvider.GetCalculator(debt.InterestType);

    //        var new_amount = calculator.Calculate(debt.StartAmount,
    //                                              debt.InterestRate,
    //                                              debt.CapitalisationsPerYear,
    //                                              debt.FixedAddition,
    //                                              debt.StartDate,
    //                                              date);

    //        var interests = new_amount - debt.TotalAmount;

    //        //interests = interests >= 0 ? interests : -interests;

    //        debt.ChargeInterest(interests);

    //        await _debtRepository.Update(debt);
    //    }
    //}
}
