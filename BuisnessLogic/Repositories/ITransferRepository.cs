using DataAccess.Entities;
namespace BuisnessLogic.Repositories
{
    public interface ITransferRepository
    {
        Task Add(Transfer transfer);
        Task Update(Transfer transfer);
    }

}
