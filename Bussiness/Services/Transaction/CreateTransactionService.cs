using System;
using AutoMapper;
using Domain.Models;
using Domain.Models.Transaction;
using Repository.DataModels;
using Repository.DataModels.Transaction;
using Repository.Interfaces;

namespace Bussiness.Services.Transaction
{
	public interface ICreateTransactionService
	{
		ReturnCreateTransactionModel CreateTransaction(CreateTransactionModel objCreateTransactionModel);

    }
	public class CreateTransactionService:ICreateTransactionService
	{
        private readonly ITransactionRepository _createTransaction;
        private readonly IMapper _mapper;
        public CreateTransactionService(ITransactionRepository createTransaction,IMapper mapper)
		{
			_mapper = mapper;
			_createTransaction = createTransaction;
		}
		public ReturnCreateTransactionModel CreateTransaction(CreateTransactionModel objCreateTransactionModel)
		{
            var dto = _mapper.Map<CreateTransactionDTO>(objCreateTransactionModel);
            var result = _createTransaction.CreateTransaction(dto);
            var objReturnCreateTransactionModel = _mapper.Map<ReturnCreateTransactionModel>(result);
            return objReturnCreateTransactionModel;
		}
	}
}

