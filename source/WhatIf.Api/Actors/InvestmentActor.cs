using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using WhatIf.Api.Models;

namespace WhatIf.Api.Actors
{
    public interface IInvestmentActor : IActor
    {
        Task Invest(InvestmentRequest investmentRequest);

        Task StopMonitoring();
    }

    public class InvestmentActor : Actor, IInvestmentActor, IRemindable
    {
        private readonly DaprClient daprClient;

        public InvestmentActor(ActorHost host, DaprClient daprClient) : base(host)
        {
            this.daprClient = daprClient;
        }

        public async Task Invest(InvestmentRequest investmentRequest)
        {
            var investmentValue = await StateManager.TryGetStateAsync<InvestmentState>("investment");
            if (!investmentValue.HasValue || (investmentRequest.Value != investmentValue.Value.Value))
            {
                Logger.LogInformation($"{Id} investment changed to {investmentRequest.Token} {investmentRequest.Value}");
                await StateManager.SetStateAsync("investment", new InvestmentState($"{investmentRequest.Token.ToUpper()}USDT", investmentRequest.Value));
                await StateManager.SaveStateAsync();
                await RegisterReminderAsync("Request-Price", null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
            }
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == "Request-Price")
            {
                var investment = await StateManager.TryGetStateAsync<InvestmentState>("investment");
                var metadata = new Dictionary<string, string>() { ["path"] = $"price?symbol={investment.Value.Pair.ToUpper()}" };
                var response = await daprClient.InvokeBindingAsync<object?, BinancePriceResponse>("binance-pair", "get", null, metadata);
                var calculation = investment.Value.Value / response.Price;
                await daprClient.PublishEventAsync("calculation", "base-investment-changed", new BaseInvestmentChanged(investment.Value.Pair.ToUpper(), investment.Value.Value, calculation));
            }
        }

        public async Task StopMonitoring()
        {
            await UnregisterReminderAsync("Request-Price");
        }
    }

}
