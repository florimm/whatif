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
    };

    public record MonitorPairRequest(string From, string To, int RefreshIntervalInSeconds = 60)
    {
        public string? From { get; init; }
        public string? To { get; init; }
        public int RefreshIntervalInSeconds {get;init;} = 60;

        public string ToPair() => $"{From?.ToUpper()}-{To?.ToUpper()}";

        public string ToPairRequest() => $"{From?.ToUpper()}{To?.ToUpper()}";

        public static MonitorPairRequest FromPair(string pair)
        {
            var parts = pair.Split('-');
            return new MonitorPairRequest(parts[0], parts[1]);
        }
    };
    
    public record PairState(string Symbol, int RefreshIntervalInSeconds);
    
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
                var result = await MakeRequest<PriceResponse>($"{from}{to}");
                return result;
            }
            catch (Exception ex)
            {
                throw new PairNotExistException();
            }
        }

        public async Task Monitor(MonitorPairRequest request)
        {                
            if (!timerRegistered)
            {
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
            await daprClient.PublishEventAsync("pubsub", "price-change", new PairPriceChanged(request.From, request.To, response.Price));
        }

        private async Task<T> MakeRequest<T>(string pair)
        {
            var metadata = new Dictionary<string, string>() { ["path"] = $"ticker/price?symbol={pair}" };
            var response = await daprClient.InvokeBindingAsync<object?, T>("binance-price", "get", null, metadata);
            return response;
        }
    }
}
