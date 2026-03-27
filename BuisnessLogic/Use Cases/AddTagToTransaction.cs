using BuisnessLogic.Repositories;

namespace BuisnessLogic.Use_Cases
{
    public class AddTagToTransaction
    {
        readonly ITransactionTagRepository _transactionTagRepository;
        readonly ITransactionRepository _transactionRepository;

        public AddTagToTransaction(ITransactionTagRepository transactionTagRepository, ITransactionRepository transactionRepository) 
        {
            _transactionTagRepository = transactionTagRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task Execute(DTO.AddTagToTransactionDTO dto)
        {
            var tag = await _transactionTagRepository.GetById(dto.TransactionTagId);
            var transaction = await _transactionRepository.GetById(dto.TransactionId);

            transaction.AddTag(tag);

            await _transactionRepository.Update(transaction);
        }
    }
}
