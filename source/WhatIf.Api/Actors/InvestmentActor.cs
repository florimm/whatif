using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using WhatIf.Api.Commands;

namespace WhatIf.Api.Actors
{
    public record InvestmentState(Guid WalletId, string Pair, double Value);

    public interface IInvestmentActor : IActor
    {
        Task Invest(AddInvestmentRequest investmentRequest);
    }

    public class InvestmentActor : Actor, IInvestmentActor
    {
        private readonly DaprClient daprClient;
        private InvestmentState? state;

        public InvestmentActor(ActorHost host, DaprClient daprClient) : base(host)
        {
            this.daprClient = daprClient;
        }

        protected override async Task OnActivateAsync()
        {
            var investmentValue = await StateManager.TryGetStateAsync<InvestmentState>("investment");
            state = investmentValue.HasValue ? investmentValue.Value :  null;
        }

        public async Task Invest(AddInvestmentRequest investmentRequest)
        {
            if (state == null || (investmentRequest.Amount != state.Value))
            {
                state = new InvestmentState(investmentRequest.WalletId, $"{investmentRequest.Symbol.ToUpper()}", investmentRequest.Amount);
                await StateManager.SetStateAsync("investment", state);
                await StateManager.SaveStateAsync();
            }
            var proxy = this.ProxyFactory.CreateActorProxy<IPairActor>(new ActorId($"{investmentRequest.Symbol.ToUpper()}"), nameof(PairActor));
            await proxy.Monitor(new MonitorPairRequest(investmentRequest.Symbol));
        }
    }

}
