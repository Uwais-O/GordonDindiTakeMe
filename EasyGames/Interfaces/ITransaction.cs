using EasyGames.Models;
using EasyGames.Services;

namespace EasyGames.Interfaces
{
    public interface ITransaction
    {
        void Create(decimal amount, int transactionTypeId, int clientId, string comment);
        Task Delete(int id);
        Task<Transaction> GetTransactionById(int id);
        IEnumerable<Transaction> GetAllTransactions();
        void UpdateComment(int id, string comment);
        void UpdateTransactionAndClientBalance(int clientId, decimal amount, TransactionTypes transactionType);
    }
}
