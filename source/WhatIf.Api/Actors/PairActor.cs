using System.Text;
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
    }

    public class PairActor : Actor, IPairActor
    {
        private readonly DaprClient daprClient;
        private bool timerRegistered = false;
        public PairActor(ActorHost host, DaprClient daprClient) : base(host)
        {
            this.daprClient = daprClient;
        }

        public async Task Monitor(MonitorPairRequest request)
        {
            if (!timerRegistered)
            {
                timerRegistered = true;
                var pairSymbol = Encoding.UTF8.GetBytes(request.Symbol.ToUpper());
                await RegisterTimerAsync("Request-Price", nameof(TimerCallbackAsync), pairSymbol, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(request.RefreshIntervalInSeconds));
            }
        }

        public async Task TimerCallbackAsync(byte[] state)
        {
            var pairSymbol = Encoding.UTF8.GetString(state);
            var metadata = new Dictionary<string, string>() { ["path"] = $"price?symbol={pairSymbol}" };
            var response = await daprClient.InvokeBindingAsync<object?, BinancePriceResponse>("binance-pair", "get", null, metadata);
            await daprClient.PublishEventAsync("pubsub", "price-change", new PairPriceChanged(response.Symbol, response.Price));
        }
    }
}
