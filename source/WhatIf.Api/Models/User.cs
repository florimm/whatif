namespace WhatIf.Api.Models
{
    public record CreateUserRequest(Guid Id, string Name, string Password);

}