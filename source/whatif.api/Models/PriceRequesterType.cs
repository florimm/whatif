namespace WhatIf.Api.Models
{
    public abstract class PriceRequesterType
    {
        public string Name { get; private set; }
        private PriceRequesterType(string name)
        {
            this.Name = name;
        }

        public static readonly BinancePriceRequester Binance = new();
        public static readonly CoinGeckoRequester CoinGecko = new();

        public static PriceRequesterType FromValue(string name)
        {
            switch (name)
            {
                case "binance":
                    return Binance;
                case "coingecko":
                    return CoinGecko;
                default:
                    throw new ArgumentException("Invalid requester type");
            }
        }
        
        public abstract Dictionary<string, string> GetMetadata(string from, string to);

        public class BinancePriceRequester : PriceRequesterType
        {
            public BinancePriceRequester() : base("binance-price")
            {
            }

            override public Dictionary<string, string> GetMetadata(string from, string to)
            {
                var metadata = new Dictionary<string, string>() { ["path"] = $"ticker/price?symbol={from.ToUpper()}{to.ToUpper()}" };
                return metadata;
            }
        }

        public class CoinGeckoRequester : PriceRequesterType
        {
            public CoinGeckoRequester() : base("gecko-price")
            {
            }

            override public Dictionary<string, string> GetMetadata(string from, string to)
            {
                var metadata = new Dictionary<string, string>() { ["path"] = $"ticker/price?symbol={from.ToUpper()}{to.ToUpper()}" };
                return metadata;
            }
        }
    }
}
