using System;

namespace Swync.core.Functional
{
    public struct Maybe<T> where T:class
    {
        private Maybe(T value)
        {
            Value = value;
        }

        public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);
        
        public bool HasValue => Value != null;

        public T Value { get; }
    }

    public static class Maybe
    {
        public static Maybe<T> WithValue<T>(T value) where T : class
        {
            if (value == null)
            {
                throw new NullReferenceException();
            }
            return value;
        }

        public static Maybe<T> None<T>() where T : class => null as T;
    }
}