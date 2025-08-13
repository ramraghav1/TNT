using System;
using Bussiness.Services.Transaction;
using Domain.Models.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController:ControllerBase
	{
        private readonly ICreateTransactionService _createTransactionservice;
        private readonly ITransactionService _transactionService;
		public TransactionController(ICreateTransactionService createTransactionservice, ITransactionService transactionService)
		{
            _createTransactionservice = createTransactionservice;
            _transactionService = transactionService;
		}
        [Route("create")]
        [HttpPost]
        public IActionResult CreateTransaction([FromBody] CreateTransactionModel objCreateTransactionModel)
        {
            var objReturn = _createTransactionservice.CreateTransaction(objCreateTransactionModel);

            if (objReturn.TransactionId > 0)
            {
                return Ok(objReturn);
            }
            else
            {
                return BadRequest();
            }
        }
        [Route("list")]
        [HttpGet]
        public IActionResult GetTransactionList(string fromDate, string toDate)
        {
            var objReturn = _transactionService.GetTransactionList();
            return Ok(objReturn);
        }
    }
}

