namespace WhatIf.Api.Models
{
    public class BinancePriceResponse
    {
        public string Symbol { get; set; }
        public double Price { get; set; } = 0;
    }
}
