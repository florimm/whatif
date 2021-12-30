using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.States;

namespace WhatIf.Api.Queries.Wallet
{
    public record WalletInvestment(string Pair, string From, string To, double Amount);
    public record GetWalletInvestmentsQueryResult(string WalletName, List<WalletInvestment> Investments);

    public record GetWalletInvestmentsQuery(Guid WalletId) : IRequest<GetWalletInvestmentsQueryResult>;

    public class GetWalletInvestmentsQueryHandler : IRequestHandler<GetWalletInvestmentsQuery, GetWalletInvestmentsQueryResult>
    {
        private readonly DaprClient daprClient;
        private readonly ClaimsPrincipal principal;

        public GetWalletInvestmentsQueryHandler(DaprClient daprClient, IPrincipal principal)
        {
            this.daprClient = daprClient;
            this.principal = (ClaimsPrincipal)principal;
        }

        public async Task<GetWalletInvestmentsQueryResult> Handle(GetWalletInvestmentsQuery request, CancellationToken cancellationToken)
        {
            string userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var walletInvestments = await daprClient.GetStateAsync<WalletInvestments>("statestore", request.WalletId.ToString());
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{userId}-wallets");
            var currentWallet = userWallets.Wallets.Single(t => t.Id == request.WalletId);
            if (walletInvestments == null)
            {
                return new GetWalletInvestmentsQueryResult(currentWallet.Name, new List<WalletInvestment>());
            }
            return new GetWalletInvestmentsQueryResult(currentWallet.Name, 
                walletInvestments.Investments.Select(t => new WalletInvestment(t.Pair, t.From, t.To, t.Amount)).ToList());
        }
    }

}