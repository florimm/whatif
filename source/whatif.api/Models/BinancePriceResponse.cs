namespace WhatIf.Api.Models
{
    public record BinancePriceResponse(string Symbol, double Price = 0);
}
