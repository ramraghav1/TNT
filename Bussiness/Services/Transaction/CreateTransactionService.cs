using System;
using AutoMapper;
using Domain.Models;
using Domain.Models.Transaction;
using Bussiness.Services.Remittance;
using Repository.DataModels;
using Repository.DataModels.Transaction;
using Repository.Interfaces;
using static Domain.Models.Remittance.Voucher;

namespace Bussiness.Services.Transaction
{
	public interface ICreateTransactionService
	{
		ReturnCreateTransactionModel CreateTransaction(CreateTransactionModel objCreateTransactionModel);
    }

	public class CreateTransactionService : ICreateTransactionService
	{
        private readonly ITransactionRepository _transactionRepo;
        private readonly IVoucherService _voucherService;
        private readonly IAgentAccountService _accountService;
        private readonly IMapper _mapper;

        public CreateTransactionService(
            ITransactionRepository transactionRepo,
            IVoucherService voucherService,
            IAgentAccountService accountService,
            IMapper mapper)
		{
			_mapper = mapper;
			_transactionRepo = transactionRepo;
            _voucherService = voucherService;
            _accountService = accountService;
		}

		public ReturnCreateTransactionModel CreateTransaction(CreateTransactionModel model)
		{
            // 1. Create the transaction record
            var dto = _mapper.Map<CreateTransactionDTO>(model);
            var result = _transactionRepo.CreateTransaction(dto);
            string txnRef = result.transaction_id.ToString();

            // 2. Deduct from sender agent account (DR) when sending
            if (model.SenderAgentId.HasValue)
            {
                try
                {
                    var accounts = _accountService.GetByAgent(model.SenderAgentId.Value);
                    if (accounts != null && accounts.Count > 0)
                    {
                        _voucherService.PostVoucher(new CreateVoucherRequest
                        {
                            EntityType = "AGENT",
                            AgentId = model.SenderAgentId,
                            AgentAccountId = accounts[0].Id,
                            Amount = model.CollectedAmount ?? 0,
                            Mode = "DR",
                            ReferenceType = "SEND",
                            ReferenceId = txnRef,
                            Description = $"Send transaction #{txnRef} - deducted from sender agent",
                            CreatedBy = "SYSTEM"
                        });
                    }
                }
                catch { /* Log but don't fail the transaction */ }
            }

            // 3. Deduct from sender branch (DR) when sending
            if (model.SenderBranchId.HasValue)
            {
                try
                {
                    _voucherService.PostVoucher(new CreateVoucherRequest
                    {
                        EntityType = "BRANCH",
                        BranchId = model.SenderBranchId,
                        AgentId = model.SenderAgentId,
                        Amount = model.CollectedAmount ?? 0,
                        Mode = "DR",
                        ReferenceType = "SEND",
                        ReferenceId = txnRef,
                        Description = $"Send transaction #{txnRef} - deducted from sender branch",
                        CreatedBy = "SYSTEM"
                    });
                }
                catch { /* Log but don't fail the transaction */ }
            }

            // 4. Credit to payout agent account (CR)
            if (model.PayoutAgentId.HasValue)
            {
                try
                {
                    var accounts = _accountService.GetByAgent(model.PayoutAgentId.Value);
                    if (accounts != null && accounts.Count > 0)
                    {
                        _voucherService.PostVoucher(new CreateVoucherRequest
                        {
                            EntityType = "AGENT",
                            AgentId = model.PayoutAgentId,
                            AgentAccountId = accounts[0].Id,
                            Amount = model.TransferAmount ?? 0,
                            Mode = "CR",
                            ReferenceType = "PAYOUT",
                            ReferenceId = txnRef,
                            Description = $"Send transaction #{txnRef} - credited to payout agent",
                            CreatedBy = "SYSTEM"
                        });
                    }
                }
                catch { /* Log but don't fail the transaction */ }
            }

            // 5. Credit to payout branch (CR)
            if (model.PayoutBranchId.HasValue)
            {
                try
                {
                    _voucherService.PostVoucher(new CreateVoucherRequest
                    {
                        EntityType = "BRANCH",
                        BranchId = model.PayoutBranchId,
                        AgentId = model.PayoutAgentId,
                        Amount = model.TransferAmount ?? 0,
                        Mode = "CR",
                        ReferenceType = "PAYOUT",
                        ReferenceId = txnRef,
                        Description = $"Send transaction #{txnRef} - credited to payout branch",
                        CreatedBy = "SYSTEM"
                    });
                }
                catch { /* Log but don't fail the transaction */ }
            }

            return _mapper.Map<ReturnCreateTransactionModel>(result);
		}
	}
}

