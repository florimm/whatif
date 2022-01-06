namespace WhatIf.Api.Models
{
    public record PriceResponse()
    {
        public string? Symbol { get; init; }
        public double? Price { get; init; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    };
}
