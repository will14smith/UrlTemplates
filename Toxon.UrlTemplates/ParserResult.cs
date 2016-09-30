using System;
using System.Collections.Generic;
using System.Linq;

namespace Toxon.UrlTemplates
{
    public static class ParserResult
    {
        internal static ParserResult<T> Success<T>(ParserState state, T result)
        {
            return new ParserResult<T>.Success(state, result);
        }
        internal static ParserResult<T> Failure<T>(ParserState state, IEnumerable<ParserError> errors)
        {
            return new ParserResult<T>.Failure(state, errors);
        }
    }

    public abstract class ParserResult<TResult>
    {
        private ParserResult(ParserState state)
        {
            State = state;
        }

        public abstract bool IsSuccess { get; }

        internal ParserState State { get; }

        public abstract TMap Map<TMap>(Func<Success, TMap> successFunc, Func<Failure, TMap> failureFunc);

        public ParserResult<TMap> Map<TMap>(Func<Success, ParserResult<TMap>> successFunc)
        {
            return Map(successFunc, x => new ParserResult<TMap>.Failure(x.State, x.Errors));
        }

        public void Match(Action<Success> successAction, Action<Failure> failureAction)
        {
            Map(x =>
            {
                successAction(x);
                return 0;
            }, x =>
            {
                failureAction(x);
                return 0;
            });
        }


        public class Success : ParserResult<TResult>
        {
            internal Success(ParserState state, TResult result) : base(state)
            {
                Result = result;
            }

            public TResult Result { get; }
            public override bool IsSuccess => true;

            public override TMap Map<TMap>(Func<Success, TMap> successFunc, Func<Failure, TMap> failureFunc)
            {
                return successFunc(this);
            }
        }
        public class Failure : ParserResult<TResult>
        {
            internal Failure(ParserState state, IEnumerable<ParserError> errors) : base(state)
            {
                Errors = errors.ToList();
            }

            public IReadOnlyCollection<ParserError> Errors { get; }
            public override bool IsSuccess => false;

            public override TMap Map<TMap>(Func<Success, TMap> successFunc, Func<Failure, TMap> failureFunc)
            {
                return failureFunc(this);
            }
        }
    }
}