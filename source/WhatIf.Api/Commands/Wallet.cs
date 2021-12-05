using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using MediatR;
using WhatIf.Api.Actors;
using WhatIf.Api.States;

namespace WhatIf.Api.Commands
{
    public class CreateWalletRequest : IRequest<Guid>
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateWalletHandler : IRequestHandler<CreateWalletRequest, Guid>
    {
        private readonly DaprClient daprClient;
        public CreateWalletHandler(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }
        public async Task<Guid> Handle(CreateWalletRequest request, CancellationToken cancellationToken)
        {
            var wallet = new Wallet(Guid.NewGuid(), request.Name, request.UserId, new());
            await daprClient.SaveStateAsync<Wallet>("db", wallet.Id.ToString(), wallet);
            return wallet.Id;
        }
    }
    
    // add investment in wallet
    public class AddInvestmentRequest : IRequest
    {
        public Guid WalletId { get; set; }
        public string Symbol { get; set; }
        public double Amount {get; set; }
    }

    public class AddInvestmentHandler : IRequestHandler<AddInvestmentRequest, MediatR.Unit>
    {
        private readonly DaprClient daprClient;
        private readonly IActorProxyFactory actorProxyFactory;

        public AddInvestmentHandler(DaprClient daprClient, IActorProxyFactory actorProxyFactory)
        {
            this.daprClient = daprClient;
            this.actorProxyFactory = actorProxyFactory;
        }

        public async Task<Unit> Handle(AddInvestmentRequest request, CancellationToken cancellationToken)
        {
            var wallet = await daprClient.GetStateAsync<Wallet>("db", request.WalletId.ToString());
            wallet.Investments.Add(request.Symbol);
            await daprClient.SaveStateAsync<Wallet>("db", request.WalletId.ToString(), wallet);
            var proxy = actorProxyFactory.CreateActorProxy<IInvestmentActor>(new ActorId($"{request.WalletId}-{request.Symbol}"), nameof(InvestmentActor));
            await proxy.Invest(request);
            return Unit.Value;
        }
    }
}