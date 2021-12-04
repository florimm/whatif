namespace WhatIf.Api.Models
{
    public record CryptoPairCalculationChanged(string Pair, double InvestedValue, double CurrentValue);
    public record BaseInvestmentChanged(string Pair, double InvestedValue, double CurrentValue);
}
