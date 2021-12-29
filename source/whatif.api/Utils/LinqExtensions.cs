namespace WhatIf.Api.Utils
{
    public static class LinqExtensions
    {
        
        public static List<T> Replace<T>(this List<T> source, Predicate<T> predicate, Func<T, T> replacement)
        {
            var index = source.FindIndex(predicate);
            if (index < 0)
            {
                return source;
            }
            source[index] = replacement(source[index]);
            return source;
        }
    }
    
}