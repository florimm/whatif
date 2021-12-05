namespace WhatIf.Api.Models
{
    public record CreateWalletRequest(Guid Id, string Name, Guid UserId);
}
