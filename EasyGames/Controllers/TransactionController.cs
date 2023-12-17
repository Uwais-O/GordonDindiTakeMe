using EasyGames.Interfaces;
using EasyGames.Models;
using EasyGames.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyGames.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransaction _transaction;

        public TransactionController(ITransaction transaction)
        {
            _transaction = transaction;
        }

        // GET: api/Transaction
        [HttpGet]
        public IActionResult GetAllTransactions()
        {
            var transactions = _transaction.GetAllTransactions();
            return Ok(transactions);
        }
        // POST: api/Transaction
        [HttpPost]
        public IActionResult CreateTransaction(int clientId, decimal amount)
        {
            if (amount < 0)
            {
                // If amount is negative, subtract from ClientBalance
                _transaction.UpdateTransactionAndClientBalance(clientId, amount, TransactionTypes.Debit);
            }
            else
            {
                // If amount is positive, add to ClientBalance
                _transaction.UpdateTransactionAndClientBalance(clientId, amount, TransactionTypes.Credit);
            }

            return Ok("Transaction created successfully");
        }

        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Transaction>> Get(int id)
        {
            var transaction = await _transaction.GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }
    

    // POST: api/Transaction/UpdateComment/5
    [HttpPost("UpdateComment/{id}")]
    public IActionResult UpdateComment(int id, [FromBody] string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            return BadRequest("Comment cannot be empty");
        }

        try
        {
            _transaction.UpdateComment(id, comment);
            return Ok("Comment updated successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


}

