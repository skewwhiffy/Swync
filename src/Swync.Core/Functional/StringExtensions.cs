namespace Swync.Core.Functional
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string source) => source.Pipe(string.IsNullOrWhiteSpace);
    }
}