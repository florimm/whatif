namespace WhatIf.Api.Exceptions
{
    public class WalletNotFoundException : Exception
    {
        public WalletNotFoundException() : base("Wallet not found")
        {
        }
    }
}