using Dapr.Client;
using MediatR;
using WhatIf.Api.States;

namespace WhatIf.Api.Queries
{
    public record Wallet(string Name, List<string> Symbols);
    public record GetAllWalletsForUserQueryResult(List<Wallet> Wallets);

    public class GetAllWalletsForUserQuery : IRequest<GetAllWalletsForUserQueryResult>
    {
        public string UserId { get; init; }
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
            var user = await daprClient.GetStateAsync<User>("db", request.UserId);
            user.Wallets
            return new GetAllWalletsForUserQueryResult(wallets);
        }
    }

}