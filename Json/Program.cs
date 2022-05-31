using System;

namespace Json
{
    public class Program
    {
        public static void TestParsing()
        {
            ParseAndPrint(JsonObject.Parse, "{}");
            ParseAndPrint(JsonObject.Parse, "{\"A\": null}");
            ParseAndPrint(JsonObject.Parse, "{\"A\": null, \"B\": 1}");
            ParseAndPrint(JsonObject.Parse, "{\"A\": null, \"B\": 1, \"C\": \"123\"}");
            ParseAndPrint(JsonObject.Parse, "{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}}");
            ParseAndPrint(JsonObject.Parse, "{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}, \"E\": { \"F\": true}, \"G\": []}");
            ParseAndPrint(JsonObject.Parse, "{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}, \"E\": { \"F\": true}, \"G\": [], \"H\": [1 ,2,3]}");

            ParseAndPrint(JsonArray.Parse, "[]");
            ParseAndPrint(JsonArray.Parse, "[[]]");
            ParseAndPrint(JsonArray.Parse, "[[], []]");
            ParseAndPrint(JsonArray.Parse, "[[], {}]");
            ParseAndPrint(JsonArray.Parse, "[ {}, {\"A\": [ null]}]");

            ParseAndPrint(JsonPrimitive.Parse, "\"A\\\"S\tD\"");

            ParseAndPrint(JsonPrimitive.Parse, "\"\\u0061\"");
            ParseAndPrint(JsonPrimitive.Parse, "true");
            ParseAndPrint(JsonPrimitive.Parse, "false");
            ParseAndPrint(JsonPrimitive.Parse, "null");
            ParseAndPrint(JsonPrimitive.Parse, "3.14");
            ParseAndPrint(JsonPrimitive.Parse, "-3.14");
        }

        public static void ParseAndPrint(Func<string, IJsonValue> parser, string input)
        {
            var json = parser(input);
            var prettify = json.ToString(JsonFormatStyle.Prettify);
            var independent = json.ToString(JsonFormatStyle.Independent);
            var minify = json.ToString(JsonFormatStyle.Minify);
            Console.WriteLine("====================");
            Console.WriteLine(prettify);
            Console.WriteLine(independent);
            Console.WriteLine(minify);

            parser(prettify);
            parser(independent);
            parser(minify);
        }

        public static void Main(string[] args)
        {
            TestParsing();

            TestPrettify();
        }

        public static void TestPrettify()
        {
            ParseAndPrint(JsonObject.Parse, @"{   ""datetime"": ""2022-06-01T13:40:00"", ""code"":
{""python"":               ""print(\""good\"")""
,""rust""
:

""print!(\""goodn\"");""}
, ""data"": [false, 1, ""    2"",
3, 4]}");

            ParseAndPrint(JsonObject.Parse, @"{""NickName"":
"",\""d:e//lu,\"""",   ""tooSad""
:
[null, 3.2253, "":raaLf;:sadF]\""soaf]]]:\""%rate""]
  
  , ""RainWar""        : ""}feel][So:,g\""OOd,[\""{"" }");
        }

    }

}
