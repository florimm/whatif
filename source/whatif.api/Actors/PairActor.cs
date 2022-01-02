using System.Text;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using WhatIf.Api.Models;

namespace WhatIf.Api.Actors
{
    public record PairPriceChanged(string Pair, double Price);
    public class MonitorPairRequest
    {
        public MonitorPairRequest(string symbol, int refreshInterval = 60)
        {
            this.Symbol = symbol;
            this.RefreshIntervalInSeconds = refreshInterval;
        }
        public MonitorPairRequest()
        {
            
        }

        public string Symbol {get;set;}
        public int RefreshIntervalInSeconds {get;set;} = 60;
    }
    
    public record PairState(string Symbol, int RefreshIntervalInSeconds);
    
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
            var pairSymbol = Encoding.UTF8.GetBytes(request.Symbol.ToUpper());
                await RegisterTimerAsync("Request-Price", nameof(TimerCallbackAsync), pairSymbol, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(request.RefreshIntervalInSeconds));
                
            // if (!timerRegistered)
            // {
            //     timerRegistered = true;
                
            // }
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
