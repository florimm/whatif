using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using WhatIf.Api.Models;

namespace WhatIf.Api.Actors
{
    public record PairPriceChanged(string Pair, double Price);
    public record MonitorPairRequest(string Symbol, int RefreshIntervalInSeconds = 60);
    
    public class PairState
    {
        public PairState()
        {
            this.CallbackActorRef = new List<string>();
        }
        public string Symbol { get; set; }

        public List<string> CallbackActorRef { get; set; }
        public int RefreshIntervalInSeconds { get; set; }
        
        public bool AreEquals(MonitorPairRequest request)
        {
            return request.Symbol == Symbol && request.RefreshIntervalInSeconds == RefreshIntervalInSeconds;
        }
    }
    public interface IPairActor : IActor
    {
        Task Monitor(MonitorPairRequest request);
        Task StopMonitoring();
    }

    public class PairActor : Actor, IPairActor, IRemindable
    {
        private readonly DaprClient daprClient;

        public PairActor(ActorHost host, DaprClient daprClient) : base(host)
        {
            this.daprClient = daprClient;
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            var pairSymbol = await StateManager.TryGetStateAsync<PairState>("pair");
            if (reminderName == "Request-Price")
            {
                var metadata = new Dictionary<string, string>() { ["path"] = $"price?symbol={pairSymbol.Value.Symbol}" };
                var response = await daprClient.InvokeBindingAsync<object?, BinancePriceResponse>("binance-pair", "get", null, metadata);
                await daprClient.PublishEventAsync("pubsub", "price-change", new PairPriceChanged(response.Symbol, response.Price));
            }
        }

        public async Task Monitor(MonitorPairRequest request)
        {
            var pairState = await StateManager.TryGetStateAsync<PairState>("pair");
            if (!pairState.HasValue || !pairState.Value.AreEquals(request))
            {
                await StateManager.SetStateAsync("pair", new PairState { Symbol = request.Symbol.ToUpper(), RefreshIntervalInSeconds = request.RefreshIntervalInSeconds });
                await StateManager.SaveStateAsync();
                await RegisterReminderAsync("Request-Price", null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(request.RefreshIntervalInSeconds));
            }
        }

        public async Task StopMonitoring()
        {
            await UnregisterReminderAsync("Request-Price");
        }
    }
}
