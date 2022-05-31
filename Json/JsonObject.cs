using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Json
{
    public class JsonObject : Dictionary<string, IJsonValue>, IJsonValue
    {
        public static JsonObject Parse(string input)
        {
            using var reader = new JsonReader(input);

            if (reader.Next() is JsonObject obj)
            {
                reader.EnsureEndWithWhitespace();
                return obj;
            }
            else
            {
                throw new ArgumentException("input is not json object");
            }

        }

        public JsonObject(IEnumerable<KeyValuePair<string, IJsonValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                this[pair.Key] = pair.Value;
            }

        }

        public string ToString(JsonFormatStyle style) => new JsonFormatter(style).Format(this);

        public override string ToString() => this.ToString(JsonFormatStyle.Prettify);

    }

}
