using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.Resolvers
{
    internal class ReflectionMappings
    {
        private static readonly ConcurrentDictionary<Type, Func<object, IValue>> TypeMappings = new ConcurrentDictionary<Type, Func<object, IValue>>();
        private static readonly ConcurrentDictionary<Tuple<Type, string>, Func<object, IValue>> TypeKeyMappings = new ConcurrentDictionary<Tuple<Type, string>, Func<object, IValue>>();

        static ReflectionMappings()
        {
            AddTypedMapping<string>(x => new ConstantStringValue(x));
        }

        private static void AddTypedMapping<T>(Func<T, IValue> mapping)
        {
            TypeMappings.TryAdd(typeof(T), x => mapping((T)x));
        }

        internal static Func<object, IValue> GetTypeMapping(Type type)
        {
            return TypeMappings.GetOrAdd(type, CreateTypeMapping);
        }
        internal static Func<object, IValue> GetTypeKeyMapping(Type type, string key)
        {
            return TypeKeyMappings.GetOrAdd(Tuple.Create(type, key), x => CreateTypeKeyMapping(x.Item1, x.Item2));
        }

        private static Func<object, IValue> CreateTypeMapping(Type type)
        {
            throw new NotImplementedException();
        }

        private static Func<object, IValue> CreateTypeKeyMapping(Type type, string field)
        {
            var o = Expression.Variable(typeof(object), "o");
            var x = Expression.TypeAs(o, type);
            var f = Expression.PropertyOrField(x, field);

            var rawTypeMapping = GetTypeMapping(f.Type);
            var callTypeMapping = Expression.Invoke(Expression.Constant(rawTypeMapping), f);

            // o => GetTypeMapping(typeof(TField))((o as T).[field])
            var outerLambda = Expression.Lambda<Func<object, IValue>>(callTypeMapping, o);

            return outerLambda.Compile();
        }
    }
}