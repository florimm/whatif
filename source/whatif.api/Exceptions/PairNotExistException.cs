namespace WhatIf.Api.Exceptions
{
    public class PairNotExistException : Exception
    {
        public PairNotExistException() : base("Pair not exist")
        {
        }
    }
}