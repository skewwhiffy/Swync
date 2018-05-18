using System;

namespace Swync.Core.Functional
{
    public static class FunctionalExtensions
    {
        public static TTo Pipe<TFrom, TTo>(this TFrom from, Func<TFrom, TTo> map) => map(from);

        public static T Pipe<T>(this T from, Action<T> action) => from.Pipe(f =>
        {
            action(f);
            return f;
        });
    }
}