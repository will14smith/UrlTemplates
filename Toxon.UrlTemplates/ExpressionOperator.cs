using System.Collections.Generic;

namespace Toxon.UrlTemplates
{
    internal class ExpressionOperator
    {
        private static readonly IReadOnlyDictionary<char, ExpressionOperator> Operators;

        static ExpressionOperator()
        {
            Operators = new Dictionary<char, ExpressionOperator>
            {
                { '\0', new ExpressionOperator(Option.None<char>(), "", ",", false, "", ExpressionEscapeMode.Unreserved) },
                { '+', new ExpressionOperator(Option.Some('+'), "", ",", false, "", ExpressionEscapeMode.UnreservedAndReserved) },
                { '.', new ExpressionOperator(Option.Some('.'), ".", ".", false, "", ExpressionEscapeMode.Unreserved) },
                { '/', new ExpressionOperator(Option.Some('/'), "/", "/",false , "", ExpressionEscapeMode.Unreserved) },
                { ';', new ExpressionOperator(Option.Some(';'), ";", ";", true, "", ExpressionEscapeMode.Unreserved) },
                { '?', new ExpressionOperator(Option.Some('?'), "?", "&", true, "=", ExpressionEscapeMode.Unreserved) },
                { '&', new ExpressionOperator(Option.Some('&'), "&", "&", true, "=", ExpressionEscapeMode.Unreserved) },
                { '#', new ExpressionOperator(Option.Some('#'), "#", ",", false, "", ExpressionEscapeMode.UnreservedAndReserved) },
            };
        }

        public static Option<ExpressionOperator> GetByChar(Option<char> op)
        {
            return op.Map(
                x => Operators.TryGet(x),
                () => Operators.TryGet('\0'));
        }

        private ExpressionOperator(Option<char> op, string first, string seperator, bool named, string ifEmpty, ExpressionEscapeMode escapeMode)
        {
            Operator = op;
            First = first;
            Seperator = seperator;
            Named = named;
            IfEmpty = ifEmpty;
            EscapeMode = escapeMode;
        }

        public Option<char> Operator { get; }

        public string First { get; }
        public string Seperator { get; }
        public bool Named { get; }
        public string IfEmpty { get; }
        public ExpressionEscapeMode EscapeMode { get; }



        public override string ToString()
        {
            return Operator.ToString();
        }
    }

    internal enum ExpressionEscapeMode
    {
        Unreserved,
        UnreservedAndReserved,
    }
}