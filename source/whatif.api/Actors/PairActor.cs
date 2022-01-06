using System.Text;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using WhatIf.Api.Exceptions;
using WhatIf.Api.Models;

namespace WhatIf.Api.Actors
{
    public record PairPriceChanged(string From, string To, double Price) {
        public string Pair => $"{From.ToUpper()}{To.ToUpper()}";
    }

    public record PriceState()
    {
        public string? Symbol { get; init; }
        public double? Price { get; init; }

        public DateTime Date { get; set; }
    }

    public class MonitorPairRequest
    {
        public MonitorPairRequest(string from, string to)
        {
            From = from;
            To = to;
        }

        public MonitorPairRequest()
        {
            
        }

        public string From { get;set;}
        public string To { get;set;}
        public int RefreshIntervalInSeconds {get;set;} = 60;
        public string ToPair() => $"{From?.ToUpper()}-{To?.ToUpper()}";

        public string ToPairRequest() => $"{From?.ToUpper()}{To?.ToUpper()}";

        public static MonitorPairRequest FromPair(string pair)
        {
            var parts = pair.Split('-');
            return new MonitorPairRequest(parts[0], parts[1]);
        }
    };
    
    public interface IPairActor : IActor
    {
        Task Monitor(MonitorPairRequest request);

        Task<PriceResponse> CurrentPrice(string from, string to);
    }

    public class PairActor : Actor, IPairActor
    {
        private readonly DaprClient daprClient;
        private bool timerRegistered = false;
        public PairActor(ActorHost host, DaprClient daprClient) : base(host)
        {
            this.daprClient = daprClient;
        }

        public async Task<PriceResponse> CurrentPrice(string from, string to)
        {
            try
            {
                System.Console.WriteLine("refresh for " + $"{from}{to}");
                var lastPrice = await StateManager.GetStateAsync<PriceState>($"{from.ToUpper()}{to.ToUpper()}");
                if (lastPrice?.Date.AddSeconds(20) > DateTime.UtcNow)
                {
                    return new PriceResponse
                    {
                        Symbol = lastPrice.Symbol,
                        Price = lastPrice.Price,
                        Date = lastPrice.Date
                    };
                }

                var result = await MakeRequest<PriceResponse>($"{from.ToUpper()}{to.ToUpper()}");
                await this.StateManager.SetStateAsync($"{from.ToUpper()}{to.ToUpper()}", new PriceState
                {
                    Symbol = result.Symbol,
                    Price = result.Price,
                    Date = DateTime.UtcNow
                });
                await SaveStateAsync();
                return result;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: " + ex.Message);
                throw new PairNotExistException();
            }
        }

        public async Task Monitor(MonitorPairRequest request)
        {                
            if (!timerRegistered)
            {
                System.Console.WriteLine("Monitoring");
                timerRegistered = true;
                var pairSymbol = Encoding.UTF8.GetBytes(request.ToPair());
                await RegisterTimerAsync(
                    "Request-Price",
                    nameof(TimerCallbackAsync),
                    pairSymbol,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(request.RefreshIntervalInSeconds));
            }
        }

        public async Task TimerCallbackAsync(byte[] state)
        {
            var pairSymbol = Encoding.UTF8.GetString(state);
            var request = MonitorPairRequest.FromPair(pairSymbol);
            var response = await MakeRequest<PriceResponse>(request.ToPairRequest());
            System.Console.WriteLine($"==================================={response.Symbol} => {response.Price}");
            await daprClient.PublishEventAsync("pubsub", "price-change", new PairPriceChanged(request.From, request.To, response.Price.GetValueOrDefault(0)));
        }

        private async Task<T> MakeRequest<T>(string pair)
        {
            var metadata = new Dictionary<string, string>() { ["path"] = $"ticker/price?symbol={pair.ToUpper()}" };
            var response = await daprClient.InvokeBindingAsync<object?, T>("binance-price", "get", null, metadata);
            return response;
        }
    }
}
