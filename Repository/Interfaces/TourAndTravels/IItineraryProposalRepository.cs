using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.ItineraryProposalDTO;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IItineraryProposalRepository
    {
        // Create a new proposal with days
        ProposalResponse CreateProposal(CreateProposalRequest request, string token);

        // Get all proposals (admin list)
        List<ProposalListItem> GetAllProposals();

        // Get proposal by ID (admin)
        ProposalResponse? GetProposalById(long id);

        // Get proposal by token (public access)
        ProposalResponse? GetProposalByToken(string token);

        // Update proposal status
        bool UpdateProposalStatus(long id, string status);

        // Submit payment for a proposal (traveler)
        ProposalPaymentResponse SubmitPayment(long proposalId, SubmitProposalPaymentRequest request);

        // Get payments for a proposal
        List<ProposalPaymentResponse> GetPayments(long proposalId);

        // Verify payment (admin)
        bool VerifyPayment(long paymentId, string verifiedBy, bool approved, string? remarks);

        // Submit feedback (traveler)
        ProposalFeedbackResponse SubmitFeedback(long proposalId, SubmitFeedbackRequest request);

        // Get feedback for a proposal
        List<ProposalFeedbackResponse> GetFeedback(long proposalId);

        // Upload payment screenshot path
        bool UpdatePaymentScreenshot(long paymentId, string screenshotPath);

        // Re-send: update proposal with new customization
        ProposalResponse? UpdateProposal(long id, CreateProposalRequest request);
    }
}
