using Dapper;
using EasyGames.Interfaces;
using EasyGames.Models;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using System.Xml.Linq;

namespace EasyGames.Services
{
    public enum TransactionTypes
    {
        Credit = 1,
        Debit = 2
    }
    public class TransactionService : ITransaction
    {
        private readonly IConfiguration configuration;
        private readonly IDbConnection _connection;
        public TransactionService(IConfiguration configuration, IDbConnection connection)
        {
            this.configuration = configuration;
            _connection = connection;
        }

        void ITransaction.Create(decimal amount, int transactionTypeId, int clientId, string comment)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("UserContext")))
            {
                string query = "INSERT INTO TransactionTable (Amount, TransactionID, ClientID, Comment) Values (@Amount, @TransactionID, @ClientID, @Comment)";
                connection.Execute(query, new { Amount = amount, TransactionTypeID = transactionTypeId, ClientID = clientId, Comment = comment });
            }
            
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.Transaction> GetTransactionById(int id)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("UserContext")))
            {
                await connection.OpenAsync();

                string query = "SELECT TransactionID, Amount, TransactionTypeID, ClientID, Comment FROM TransactionsTable WHERE TransactionID = @TransactionId";
                var transaction = await connection.QueryFirstOrDefaultAsync<Models.Transaction>(query, new { TransactionId = id });

                return transaction;
            }
        }

        public IEnumerable<Models.Transaction> GetAllTransactions()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("UserContext")))
            {
                connection.Open();

                string query = "SELECT TransactionID, Amount, TransactionTypeID, ClientID, Comment FROM TransactionsTable";
                var transations = connection.Query<Models.Transaction>(query);

                return transations;
            }
        }

        public void UpdateComment(int id, string comment)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("UserContext")))
            {
                connection.Open();

                string query = "UPDATE TransactionsTable SET Comment = @Comment WHERE TransactionID = @TransactionId";
                connection.Execute(query, new { Comment = comment, TransactionId = id });
            }
        }

        public void UpdateTransactionAndClientBalance(int clientId, decimal amount, TransactionTypes transactionType)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Insert transaction
                    string insertQuery = "INSERT INTO TransactionsTable (Amount, TransactionTypeID, ClientID) VALUES (@Amount, @TransactionTypeID, @ClientID)";
                    _connection.Execute(insertQuery, new { Amount = amount, TransactionTypeID = (int)transactionType, ClientID = clientId }, transaction);

                    // Update client balance
                    string updateBalanceQuery = "UPDATE ClientTable SET ClientBalance = ClientBalance + @Amount WHERE ClientID = @ClientID";
                    _connection.Execute(updateBalanceQuery, new { Amount = amount, ClientID = clientId }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
