using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json
{
    public class JsonPrimitive : IJsonValue
    {
        public static string EscapedStringDelimiter { get; } = $"{JsonReader.StringEscpace}{JsonReader.StringDelimiter}";

        public static JsonPrimitive Parse(string input)
        {
            using var reader = new JsonReader(input);

            if (reader.Next() is JsonPrimitive primitive)
            {
                reader.EnsureEndWithWhitespace();
                return primitive;
            }
            else
            {
                throw new ArgumentException("input is not json primitive");
            }

        }

        public static string EscapeString(string text)
        {
            var escape = text.Replace(JsonReader.StringDelimiterString, EscapedStringDelimiter);
            return $"{JsonReader.StringDelimiterString}{escape}{JsonReader.StringDelimiterString}";
        }

        public object Value { get; set; }

        public string ToString(JsonFormatStyle style) => this.ToString();

        public override string ToString()
        {
            var value = this.Value;

            if (value is null) return JsonReader.Null;
            else if (value is bool b) return b ? JsonReader.True : JsonReader.False;
            else if (value is string str) return EscapeString(str);
            else return string.Concat(value);
        }

    }

}
