using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using WhatIf.Api.Models;

namespace WhatIf.Api.Actors
{
    public interface ICryptoPair : IActor
    {
        Task SetCrypto(string cryptoToken);

        Task OnChangeInvestment(double value);

        Task StopMonitoring();
    }

    public class CryptoPair : Actor, ICryptoPair, IRemindable
    {
        private readonly DaprClient daprClient;

        public CryptoPair(ActorHost host, DaprClient daprClient) : base(host)
        {
            this.daprClient = daprClient;
        }

        public async Task OnChangeInvestment(double value)
        {
            var baseInvestment = await StateManager.TryGetStateAsync<double>("investment");
            if (!baseInvestment.HasValue || (value != baseInvestment.Value))
            {
                Logger.LogInformation($"{Id} investment changed to {value}");
                await StateManager.SetStateAsync("investment", value);
                await StateManager.SaveStateAsync();
            }
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            var cryptPairName = await StateManager.TryGetStateAsync<string>("cryptPair");
            if (reminderName == "Request-Price")
            {
                var metadata = new Dictionary<string, string>() { ["path"] = $"price?symbol={cryptPairName.Value.ToUpper()}" };
                var response = await daprClient.InvokeBindingAsync<object?, BinancePriceResponse>("binance-pair", "get", null, metadata);
                var baseInvestment = await StateManager.TryGetStateAsync<double>("investment");                
                var calculation = baseInvestment.Value / response.Price;
                await daprClient.PublishEventAsync("calculation", "value-changed", new CryptoPairCalculationChanged(cryptPairName.Value, baseInvestment.Value, calculation));
            }
        }

        public async Task SetCrypto(string cryptoPair)
        {
            var cryptTokenName = await StateManager.TryGetStateAsync<string>("cryptPair");
            if (!cryptTokenName.HasValue || (cryptoPair != cryptTokenName.Value))
            {
                Logger.LogInformation($"{Id} cryptToken changed to {cryptTokenName}");
                await StateManager.SetStateAsync("baseInvestment", cryptoPair.ToUpper());
                await StateManager.SaveStateAsync();
                await RegisterReminderAsync("Request-Price", null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
            }
        }

        public async Task StopMonitoring()
        {
            await UnregisterReminderAsync("Request-Price");
        }
    }
}
