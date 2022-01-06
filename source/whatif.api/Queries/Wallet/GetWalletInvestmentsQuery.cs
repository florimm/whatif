using Dapr.Client;
using MediatR;
using WhatIf.Api.Services;
using WhatIf.Api.States;

namespace WhatIf.Api.Queries.Wallet
{
    public record GetWalletInvestmentsQueryResult(string WalletName, List<Investment> Investments);

    public record GetWalletInvestmentsQuery(Guid WalletId) : IRequest<GetWalletInvestmentsQueryResult>;

    public class GetWalletInvestmentsQueryHandler : IRequestHandler<GetWalletInvestmentsQuery, GetWalletInvestmentsQueryResult>
    {
        private readonly DaprClient daprClient;
        private readonly ICurrentUserService currentUser;

        public GetWalletInvestmentsQueryHandler(DaprClient daprClient, ICurrentUserService currentUser)
        {
            this.daprClient = daprClient;
            this.currentUser = currentUser;
        }

        public async Task<GetWalletInvestmentsQueryResult> Handle(GetWalletInvestmentsQuery request, CancellationToken cancellationToken)
        {
            string userId = currentUser.GetUserId();
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{userId}-wallets");
            var currentWallet = userWallets.Wallets.Single(t => t.Id == request.WalletId);
            System.Console.WriteLine($"request.WalletId.ToString() {request.WalletId}");

            var walletInvestments = await daprClient.GetStateEntryAsync<WalletInvestments>("statestore", request.WalletId.ToString());
            
            if (walletInvestments.Value is null)
            {
                return new GetWalletInvestmentsQueryResult(currentWallet.Name, new List<Investment>());
            }
            return new GetWalletInvestmentsQueryResult(currentWallet.Name, 
                walletInvestments.Value.Investments.Select(t => new Investment(t.From, t.To, t.Value, t.Amount)).ToList());
        }
    }

}