using Dapr.Client;
using MediatR;
using WhatIf.Api.Services;
using WhatIf.Api.States;

namespace WhatIf.Api.Queries.Wallet
{
    public record Wallet(Guid Id, string Name);
    public record GetAllWalletsForUserQueryResult(List<Wallet> Wallets);

    public record GetAllWalletsForUserQuery() : IRequest<GetAllWalletsForUserQueryResult>;

    public class GetAllWalletsForUserQueryHandler : IRequestHandler<GetAllWalletsForUserQuery, GetAllWalletsForUserQueryResult>
    {
        private readonly DaprClient daprClient;
        private readonly ICurrentUserService currentUserService;

        public GetAllWalletsForUserQueryHandler(DaprClient daprClient, ICurrentUserService currentUserService)
        {
            this.daprClient = daprClient;
            this.currentUserService = currentUserService;
        }

        public async Task<GetAllWalletsForUserQueryResult> Handle(GetAllWalletsForUserQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.GetUserId();
            var userWallets = await daprClient.GetStateAsync<UserWallets>("statestore", $"{userId}-wallets");
            if (userWallets == null)
            {
                return new GetAllWalletsForUserQueryResult(new List<Wallet>());
            }
            return new GetAllWalletsForUserQueryResult(userWallets.Wallets.Select(t => new Wallet(t.Id, t.Name)).ToList());
        }
    }
}