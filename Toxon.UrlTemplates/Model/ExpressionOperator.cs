using System.Collections.Generic;
using Toxon.UrlTemplates.Utils;

namespace Toxon.UrlTemplates.Model
{
    internal class ExpressionOperator
    {
        private static readonly IReadOnlyDictionary<char, ExpressionOperator> Operators;
        private static readonly IReadOnlyDictionary<ExpressionOperator, ExpressionOperator> NextOperator;

        static ExpressionOperator()
        {
            Operators = new Dictionary<char, ExpressionOperator>
            {
                {'\0', new ExpressionOperator(Option.None<char>(), "", false, "", ExpressionEscapeMode.Unreserved)},
                {'+', new ExpressionOperator(Option.Some('+'), "", false, "", ExpressionEscapeMode.UnreservedAndReserved)},
                {'.', new ExpressionOperator(Option.Some('.'), ".", false, "", ExpressionEscapeMode.Unreserved)},
                {'/', new ExpressionOperator(Option.Some('/'), "/", false, "", ExpressionEscapeMode.Unreserved)},
                {';', new ExpressionOperator(Option.Some(';'), ";", true, "", ExpressionEscapeMode.Unreserved)},
                {'?', new ExpressionOperator(Option.Some('?'), "?", true, "=", ExpressionEscapeMode.Unreserved)},
                {'&', new ExpressionOperator(Option.Some('&'), "&", true, "=", ExpressionEscapeMode.Unreserved)},
                {'#', new ExpressionOperator(Option.Some('#'), "#", false, "", ExpressionEscapeMode.UnreservedAndReserved)}
            };

            var commaUnreserved = new ExpressionOperator(Option.Some(','), ",", false, "", ExpressionEscapeMode.Unreserved);
            var commaUnreservedReserved = new ExpressionOperator(Option.Some(','), ",", false, "", ExpressionEscapeMode.UnreservedAndReserved);

            NextOperator = new Dictionary<ExpressionOperator, ExpressionOperator>
            {
                {Operators['\0'], commaUnreserved},
                {Operators['+'], commaUnreservedReserved},
                {Operators['.'], Operators['.']},
                {Operators['/'], Operators['/']},
                {Operators[';'], Operators[';']},
                {Operators['?'], Operators['&']},
                {Operators['&'], Operators['&']},
                {Operators['#'], commaUnreservedReserved},
                {commaUnreserved, commaUnreserved},
                {commaUnreservedReserved, commaUnreservedReserved}
            };
        }

        private ExpressionOperator(Option<char> op, string prefix, bool named, string ifEmpty,
            ExpressionEscapeMode escapeMode)
        {
            Operator = op;
            Prefix = prefix;
            Named = named;
            IfEmpty = ifEmpty;
            EscapeMode = escapeMode;
        }

        public Option<char> Operator { get; }

        public string Prefix { get; }
        public bool Named { get; }
        public string IfEmpty { get; }
        public ExpressionEscapeMode EscapeMode { get; }

        public static Option<ExpressionOperator> GetByChar(Option<char> op)
        {
            return op.Map(
                x => Operators.TryGet(x),
                () => Operators.TryGet('\0'));
        }

        public static ExpressionOperator GetNextOperator(ExpressionOperator op)
        {
            return NextOperator[op];
        }

        public override string ToString()
        {
            return Operator.Map(x => x.ToString(), () => "");
        }
    }
}