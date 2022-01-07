using System.Text;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using WhatIf.Api.Models;

namespace WhatIf.Api.Actors
{
    public record PairPriceChanged(string From, string To, double Price) {
        public string Pair => $"{From.ToUpper()}{To.ToUpper()}";
    }

    public class MonitorPairRequest
    {
        public MonitorPairRequest(string priceSource, string from, string to)
        {
            From = from;
            To = to;
            PriceSource = priceSource;
        }

        public MonitorPairRequest()
        {
            
        }

        public string From { get;set;}
        public string To { get;set;}
        public string PriceSource { get; set; }
        public int RefreshIntervalInSeconds {get;set;} = 60;

        public static MonitorPairRequest FromBytes(byte[] data)
        {
            var fields = Encoding.UTF8.GetString(data).Split('-');
            return new MonitorPairRequest(fields[0], fields[1], fields[2]);
        }
        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes($"{PriceSource}-{From?.ToUpper()}-{To?.ToUpper()}");
        }
    };
    
    public interface IMonitorActor : IActor
    {
        Task Monitor(MonitorPairRequest request);
    }

    public class MonitorActor : Actor, IMonitorActor
    {
        private readonly DaprClient daprClient;
        private bool timerRegistered = false;
        public MonitorActor(ActorHost host, DaprClient daprClient) : base(host)
        {
            this.daprClient = daprClient;
        }

        public async Task Monitor(MonitorPairRequest request)
        {                
            if (!timerRegistered)
            {
                System.Console.WriteLine("Monitoring");
                timerRegistered = true;
                await RegisterTimerAsync(
                    "Request-Price",
                    nameof(TimerCallbackAsync),
                    request.ToBytes(),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(request.RefreshIntervalInSeconds));
            }
        }

        public async Task TimerCallbackAsync(byte[] state)
        {
            var request = MonitorPairRequest.FromBytes(state);
            var response = await MakeRequest(request);
            await daprClient.PublishEventAsync("pubsub", "price-change", new PairPriceChanged(request.From, request.To, response.Price.GetValueOrDefault(0)));
        }

        private async Task<PriceResponse> MakeRequest(MonitorPairRequest request)
        {
            var priceSourceType = PriceRequesterType.FromValue(request.PriceSource);
            var metadata = priceSourceType.GetMetadata(request.From, request.To);
            var response = await daprClient.InvokeBindingAsync<object?, PriceResponse>(priceSourceType.Name, "get", null, metadata);
            return response;
        }
    }
}
