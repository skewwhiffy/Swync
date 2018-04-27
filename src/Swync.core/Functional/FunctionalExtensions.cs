using System;

namespace Swync.core.Functional
{
    public static class FunctionalExtensions
    {
        public static TTo Pipe<TFrom, TTo>(this TFrom from, Func<TFrom, TTo> map) => map(from);
    }
}