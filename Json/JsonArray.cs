using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json
{
    public class JsonArray : List<IJsonValue>, IJsonValue
    {
        public static JsonArray Parse(string input)
        {
            using var reader = new JsonReader(input);

            if (reader.Next() is JsonArray array)
            {
                reader.EnsureEndWithWhitespace();
                return array;
            }
            else
            {
                throw new ArgumentException("input is not json array");
            }

        }

        public JsonArray(IEnumerable<IJsonValue> values)
        {
            foreach (var value in values)
            {
                this.Add(value);
            }

        }

        public string ToString(JsonFormatStyle style) => new JsonFormatter(style).Format(this);

        public override string ToString() => this.ToString(JsonFormatStyle.Prettify);

    }

}
