﻿using System;

namespace Toxon.UrlTemplates
{
    public struct Option<T>
    {
        public bool HasValue { get; }
        public T Value { get; }

        public Option(T value) { HasValue = true; Value = value; }

        public TOut Map<TOut>(Func<T, TOut> some, Func<TOut> none)
        {
            return HasValue ? some(Value) : none();
        }
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(value);
        }
        public static Option<T> None<T>()
        {
            return new Option<T>();
        }

        public static T OrElse<T>(this Option<T> opt, Func<T> func)
        {
            return opt.Map(x => x, func);
        }
    }
}