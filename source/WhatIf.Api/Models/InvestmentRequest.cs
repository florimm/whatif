namespace WhatIf.Api.Models
{
    public record InvestmentRequest(string Token, double Value);

    public record InvestmentState(string Pair, double Value);
}
