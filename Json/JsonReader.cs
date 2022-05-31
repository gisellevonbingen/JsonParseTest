using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Json
{
    public class JsonReader : IDisposable
    {
        public const char ObjectPrefix = '{';
        public const char ObjectSuffix = '}';
        public const char ArrayPrefix = '[';
        public const char ArraySuffix = ']';
        public const char StringDelimiter = '\"';
        public const char NameDelimiter = ':';
        public const char ValueSeparator = ',';

        public const string True = "true";
        public const string False = "false";
        public const string Null = "null";

        public static Regex HexDigitsPattern { get; } = new Regex("[0-9a-fA-F]");

        public static bool IsControl(char c)
        {
            if (c == ObjectPrefix || c == ObjectSuffix)
            {
                return true;
            }
            else if (c == ArrayPrefix || c == ArraySuffix)
            {
                return true;
            }
            else if (c == ValueSeparator || c == NameDelimiter)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsWhitespaceOrControl(char c) => char.IsWhiteSpace(c) || IsControl(c);

        protected TextReader Reader { get; private set; }
        protected bool LeaveOpen { get; private set; }

        public JsonReader(string input) : this(new StringReader(input))
        {

        }

        public JsonReader(TextReader reader) : this(reader, false)
        {

        }

        public JsonReader(TextReader reader, bool leaveOpen)
        {
            this.Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.LeaveOpen = leaveOpen;
        }

        protected bool TryPeek(out char value)
        {
            var c = this.Reader.Peek();
            value = (char)c;
            return c > -1;
        }

        protected bool TryRead(out char value)
        {
            var c = this.Reader.Read();
            value = (char)c;
            return c > -1;
        }

        protected bool ReadTo(char delimiter)
        {
            while (this.TryPeek(out var c) == true)
            {
                if (c == delimiter)
                {
                    this.Reader.Read();
                    return true;
                }
                else if (IsWhitespaceOrControl(c) == true)
                {
                    this.Reader.Read();
                    continue;
                }
                else
                {
                    throw new JsonSyntaxException($"Unexpected char '{c}'");
                }

            }

            throw new JsonSyntaxException($"Delimiter char '{delimiter}' not found");
        }

        public void EnsureEndWithWhitespace()
        {
            while (this.TryPeek(out var c) == true)
            {
                if (char.IsWhiteSpace(c) == true)
                {
                    this.Reader.Read();
                }
                else
                {
                    throw new JsonSyntaxException($"Unexpected char '{c}'");
                }

            }

        }

        public IJsonValue Next()
        {
            while (this.TryPeek(out var c) == true)
            {
                if (c == ObjectPrefix)
                {
                    return this.NextObject();
                }
                else if (c == ArrayPrefix)
                {
                    return this.NextArray();
                }
                else if (char.IsWhiteSpace(c) == true)
                {
                    this.Reader.Read();
                    continue;
                }
                else if (IsControl(c) == true)
                {
                    throw new JsonSyntaxException($"Unexpected char 's{c}'");
                }
                else
                {
                    return this.NextPrimitive();
                }

            }

            throw new JsonSyntaxException("Unexpected input");
        }

        protected JsonObject NextObject()
        {
            if (this.ReadTo(ObjectPrefix) == false)
            {
                throw new JsonSyntaxException("Object prefix not found");
            }

            var complete = false;
            var map = new Dictionary<string, IJsonValue>();
            var beComma = false;

            while (this.TryPeek(out var c) == true)
            {
                if (char.IsWhiteSpace(c) == true)
                {
                    this.Reader.Read();
                    continue;
                }
                else if (c == ObjectSuffix)
                {
                    this.Reader.Read();
                    complete = true;
                    break;
                }

                if (beComma == true)
                {
                    if (c == ValueSeparator)
                    {
                        beComma = false;
                        this.Reader.Read();
                        continue;
                    }
                    else
                    {
                        throw new JsonSyntaxException($"Unexpected char '{c}'");
                    }

                }

                if (c == StringDelimiter)
                {
                    var key = this.NextString();
                    this.ReadTo(NameDelimiter);
                    var value = this.Next();

                    map[key] = value;
                    beComma = true;
                }
                else
                {
                    throw new JsonSyntaxException($"Unexpected char '{c}'");
                }

            }

            if (complete == false)
            {
                throw new JsonSyntaxException($"Unfinished reading object");
            }

            return new JsonObject(map);
        }

        protected JsonArray NextArray()
        {
            if (this.ReadTo(ArrayPrefix) == false)
            {
                throw new JsonSyntaxException("Array prefix not found");
            }

            var complete = false;
            var list = new List<IJsonValue>();
            var beComma = false;

            while (this.TryPeek(out var c) == true)
            {
                if (char.IsWhiteSpace(c) == true)
                {
                    this.Reader.Read();
                    continue;
                }
                else if (c == ArraySuffix)
                {
                    this.Reader.Read();
                    complete = true;
                    break;
                }

                if (beComma == true)
                {
                    if (c == ValueSeparator)
                    {
                        beComma = false;
                        this.Reader.Read();
                        continue;
                    }
                    else
                    {
                        throw new JsonSyntaxException($"Unexpected char '{c}'");
                    }

                }

                list.Add(this.Next());
                beComma = true;
            }

            if (complete == false)
            {
                throw new JsonSyntaxException($"Unfinished reading object");
            }

            return new JsonArray(list);
        }

        protected JsonPrimitive NextPrimitive()
        {
            var builder = new StringBuilder();
            var parsing = false;

            while (this.TryPeek(out var c) == true)
            {
                if (IsWhitespaceOrControl(c) == true)
                {
                    if (parsing == true)
                    {
                        break;
                    }
                    else
                    {
                        this.Reader.Read();
                        continue;
                    }

                }

                if (c == StringDelimiter)
                {
                    if (parsing == false)
                    {
                        return new JsonPrimitive() { Value = this.NextString() };
                    }
                    else
                    {
                        throw new JsonSyntaxException($"Unexpected char {c}");
                    }

                }
                else
                {
                    parsing = true;
                    builder.Append(c);
                    this.Reader.Read();
                }

            }

            var toSting = builder.ToString();

            if (toSting.Equals(True, StringComparison.OrdinalIgnoreCase) == true)
            {
                return new JsonPrimitive() { Value = true };
            }
            else if (toSting.Equals(False, StringComparison.OrdinalIgnoreCase) == true)
            {
                return new JsonPrimitive() { Value = false };
            }
            else if (toSting.Equals(Null, StringComparison.OrdinalIgnoreCase) == true)
            {
                return new JsonPrimitive() { Value = null };
            }
            else if (double.TryParse(toSting, out var result) == true)
            {
                return new JsonPrimitive() { Value = result };
            }
            else
            {
                throw new JsonSyntaxException($"Unexpected string {toSting}");
            }

        }

        protected string NextString()
        {
            var enter = false;
            var escaping = false;
            StringBuilder hexDigits = null;
            var builder = new StringBuilder();

            while (this.TryRead(out var c) == true)
            {
                if (enter == false)
                {
                    if (c == StringDelimiter)
                    {
                        enter = true;
                    }
                    else if (IsWhitespaceOrControl(c) == false)
                    {
                        throw new JsonSyntaxException("String prefix not found");
                    }

                }
                else if (escaping == true)
                {
                    if (hexDigits != null)
                    {
                        if (HexDigitsPattern.IsMatch(c.ToString()) == true)
                        {
                            hexDigits.Append(c);

                            if (hexDigits.Length == 4)
                            {
                                var digit = Convert.ToInt16(hexDigits.ToString(), 16);
                                builder.Append((char)digit);

                                hexDigits = null;
                                escaping = false;
                            }

                        }
                        else
                        {
                            throw new JsonSyntaxException($"Unexpected char '{c}' on reading string hex digits");
                        }

                    }
                    else if (c == StringDelimiter || c == '\\')
                    {
                        builder.Append(c);
                        escaping = false;
                    }
                    else if (c == 'b')
                    {
                        builder.Append('\b');
                        escaping = false;
                    }
                    else if (c == 'f')
                    {
                        builder.Append('\f');
                        escaping = false;
                    }
                    else if (c == 'n')
                    {
                        builder.Append('\n');
                        escaping = false;
                    }
                    else if (c == 'r')
                    {
                        builder.Append('\r');
                        escaping = false;
                    }
                    else if (c == 't')
                    {
                        builder.Append('\t');
                        escaping = false;
                    }
                    else if (c == 'u')
                    {
                        hexDigits = new StringBuilder();
                    }
                    else
                    {
                        throw new JsonSyntaxException($"Unexpected char '{c}' on reading string escapce");
                    }

                }
                else if (c == '\\')
                {
                    escaping = true;
                }
                else if (c == StringDelimiter)
                {
                    enter = false;
                    break;
                }
                else
                {
                    builder.Append(c);
                }

            }

            if (enter == true)
            {
                throw new JsonSyntaxException($"Unfinished reading string");
            }
            else if (hexDigits != null)
            {
                throw new JsonSyntaxException($"Unfinished reading string's hex digits");
            }
            else if (escaping == true)
            {
                throw new JsonSyntaxException($"Unfinished reading string's escape char");
            }

            return builder.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.LeaveOpen == false)
            {
                this.Reader.Dispose();
            }

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        ~JsonReader()
        {
            this.Dispose(false);
        }

    }

}
