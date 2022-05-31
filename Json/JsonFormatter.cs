using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualBasic;

namespace Json
{
    public class JsonFormatter
    {
        public JsonFormatStyle Style { get; }

        public JsonFormatter(JsonFormatStyle style)
        {
            this.Style = style;
        }

        public string Format(IJsonValue json) => this.Format(json, 0);

        protected string Format(IJsonValue json, int level)
        {
            var prefix = this.Style == JsonFormatStyle.Prettify ? new string(' ', level * 2) : " ";
            var prefix2 = this.Style == JsonFormatStyle.Prettify ? new string(' ', (level + 1) * 2) : " ";

            if (json is null)
            {
                throw new ArgumentNullException(nameof(json));
            }
            else if (json is JsonObject obj)
            {
                var pairs = obj.ToArray();
                var builder = new StringBuilder();
                builder.Append($"{JsonReader.ObjectPrefix}");

                if (pairs.Length == 0)
                {
                    builder.Append(' ').Append(JsonReader.ObjectSuffix);
                }
                else
                {
                    if (this.Style == JsonFormatStyle.Prettify)
                    {
                        builder.AppendLine();
                    }

                    for (var i = 0; i < pairs.Length; i++)
                    {
                        var pair = pairs[i];
                        builder.Append(prefix2).Append($"{JsonReader.StringDelimiter}{pair.Key}{JsonReader.StringDelimiter}: {this.Format(pair.Value, level + 1)}");
                        this.AppendValueSeparator(builder, i, pairs.Length);
                    }

                    builder.Append($"{prefix}{JsonReader.ObjectSuffix}");
                }

                return builder.ToString();
            }
            else if (json is JsonArray array)
            {
                var values = array.ToArray();
                var builder = new StringBuilder();
                builder.Append(JsonReader.ArrayPrefix);

                if (values.Length == 0)
                {
                    builder.Append(' ').Append(JsonReader.ArraySuffix);
                }
                else
                {
                    if (this.Style == JsonFormatStyle.Prettify)
                    {
                        builder.AppendLine();
                    }

                    for (var i = 0; i < values.Length; i++)
                    {
                        var value = values[i];
                        builder.Append(prefix2).Append(this.Format(value, level + 1));
                        this.AppendValueSeparator(builder, i, values.Length);
                    }

                    builder.Append($"{prefix}{JsonReader.ArraySuffix}");
                }

                return builder.ToString();

            }
            else if (json is JsonPrimitive primitive)
            {
                return string.Concat(primitive);
            }
            else
            {
                throw new ArgumentException("Unknown json value");
            }

        }

        private void AppendValueSeparator(StringBuilder builder, int index, int length)
        {
            if (index + 1 < length)
            {
                if (this.Style == JsonFormatStyle.Prettify)
                {
                    builder.AppendLine($"{JsonReader.ValueSeparator}");
                }
                else
                {
                    builder.Append($"{JsonReader.ValueSeparator}");
                }

            }
            else if (this.Style == JsonFormatStyle.Prettify)
            {
                builder.AppendLine();
            }

        }

    }

}
