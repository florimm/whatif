using Dapr.Client;
using MediatR;
using WhatIf.Api.States;

namespace WhatIf.Api.Queries
{
    public record Wallet(Guid Id, string Name);
    public record GetAllWalletsForUserQueryResult(List<Wallet> Wallets);

    public class GetAllWalletsForUserQuery : IRequest<GetAllWalletsForUserQueryResult>
    {
        public string Email { get; init; }
    }

    //handler
    public class GetAllWalletsForUserQueryHandler : IRequestHandler<GetAllWalletsForUserQuery, GetAllWalletsForUserQueryResult>
    {
        private readonly DaprClient daprClient;

        public GetAllWalletsForUserQueryHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }

        public async Task<GetAllWalletsForUserQueryResult> Handle(GetAllWalletsForUserQuery request, CancellationToken cancellationToken)
        {
            var userWallets = await daprClient.GetStateAsync<UserWallets>("db", $"{request.Email}-wallets");
            return new GetAllWalletsForUserQueryResult(userWallets.Wallets.Select(t => new Wallet(t.Id, t.Name)).ToList());
        }
    }

}