namespace WhatIf.Api.States
{
    public record User(Guid Id, string Email, string Password, string Name, string Surname);
    public record UserWallets(string Email, List<Wallet> Wallets);
    public record Wallet(Guid Id, string Name);
    public record WalletInvestments(Guid Id, List<Investment> Investments);
    public record Investment(string Pair, double Amount);
}