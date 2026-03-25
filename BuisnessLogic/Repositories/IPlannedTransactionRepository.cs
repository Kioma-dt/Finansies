namespace BuisnessLogic.Repositories
{
    public interface IPlannedTransactionRepository
    {
        Task Add(BuisnessLogic.Entities.PlannedTransaction plannedTransaction);
        Task Update(BuisnessLogic.Entities.PlannedTransaction plannedTransaction);
    }

}
