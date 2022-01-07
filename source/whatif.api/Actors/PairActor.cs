using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using WhatIf.Api.Exceptions;
using WhatIf.Api.Models;

namespace WhatIf.Api.Actors
{
    public record PriceState()
    {
        public string? Symbol { get; init; }
        public double? Price { get; init; }

        public DateTime Date { get; set; }
    }
    
    public interface IPairActor : IActor
    {
        Task<PriceResponse> CurrentPrice(string priceSource, string from, string to);
    }

    public class PairActor : Actor, IPairActor
    {
        private readonly DaprClient daprClient;
        public PairActor(ActorHost host, DaprClient daprClient) : base(host)
        {
            this.daprClient = daprClient;
        }

        public async Task<PriceResponse> CurrentPrice(string priceSource, string from, string to)
        {
            try
            {
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

                var result = await MakeRequest(from.ToUpper(), to.ToUpper());
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
                Console.WriteLine("Error: " + ex.Message);
                throw new PairNotExistException();
            }
        }

        private async Task<PriceResponse> MakeRequest(string from, string to)
        {
            var priceSourceType = PriceRequesterType.FromValue("binance");
            var metadata = priceSourceType.GetMetadata(from, to);
            var response = await daprClient.InvokeBindingAsync<object?, PriceResponse>(priceSourceType.Name, "get", null, metadata);
            return response;
        }
    }
}
