using Dapr.Client;
using MediatR;
using WhatIf.Api.States;

namespace WhatIf.Api.Queries.Wallet
{
    public record Wallet(Guid Id, string Name);
    public record GetAllWalletsForUserQueryResult(List<Wallet> Wallets);

    public record GetAllWalletsForUserQuery(string UserId) : IRequest<GetAllWalletsForUserQueryResult>;

    public class GetAllWalletsForUserQueryHandler : IRequestHandler<GetAllWalletsForUserQuery, GetAllWalletsForUserQueryResult>
    {
        private readonly DaprClient daprClient;

        public GetAllWalletsForUserQueryHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }

        public async Task<GetAllWalletsForUserQueryResult> Handle(GetAllWalletsForUserQuery request, CancellationToken cancellationToken)
        {
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{request.UserId}-wallets");
            if (userWallets == null)
            {
                return new GetAllWalletsForUserQueryResult(new List<Wallet>());
            }
            return new GetAllWalletsForUserQueryResult(userWallets.Wallets.Select(t => new Wallet(t.Id, t.Name)).ToList());
        }
    }
}