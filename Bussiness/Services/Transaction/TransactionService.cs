using System;
using AutoMapper;
using Domain.Models.Transaction;
using Repository.Interfaces;

namespace Bussiness.Services.Transaction
{
    public interface ITransactionService
    {
        List<TransactionDetailModel> GetTransactionList();
    }
	public class TransactionService: ITransactionService
    {
        private readonly ITransactionRepository _transaction;
        private readonly IMapper _mapper;
        public TransactionService(ITransactionRepository transaction, IMapper mapper)
        {
            _mapper = mapper;
            _transaction = transaction;
        }

        public List<TransactionDetailModel> GetTransactionList()
        {
            var result = _transaction.GetTransactionList();
            var returnList=_mapper.Map<List<TransactionDetailModel>>(result);
            return returnList;
        }
		
		}
	}


