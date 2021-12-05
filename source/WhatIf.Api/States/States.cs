namespace WhatIf.Api.States
{
    public record User(Guid Id, string Email, string Password, string Name, string Surname);

     public record Wallet(Guid Id, string Name, Guid UserId, List<string> Investments);
}