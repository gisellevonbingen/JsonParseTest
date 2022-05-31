﻿using System;

namespace Json
{
    public class Program
    {
        public static void ParseTest()
        {
            Console.WriteLine(JsonObject.Parse("{}"));
            Test(JsonObject.Parse, "{}");
            Test(JsonObject.Parse, "{\"A\": null}");
            Test(JsonObject.Parse, "{\"A\": null, \"B\": 1}");
            Test(JsonObject.Parse, "{\"A\": null, \"B\": 1, \"C\": \"123\"}");
            Test(JsonObject.Parse, "{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}}");
            Test(JsonObject.Parse, "{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}, \"E\": { \"F\": true}, \"G\": []}");
            Test(JsonObject.Parse, "{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}, \"E\": { \"F\": true}, \"G\": [], \"H\": [1 ,2,3]}");


            Test(JsonArray.Parse, "[]");
            Test(JsonArray.Parse, "[[]]");
            Test(JsonArray.Parse, "[[], []]");
            Test(JsonArray.Parse, "[[], {}]");
            Test(JsonArray.Parse, "[ {}, {\"A\": [ null]}]");

            Test(JsonPrimitive.Parse, "\"A\\\"S\tD\"");

            Test(JsonPrimitive.Parse, "\"\\u0061\"");
            Test(JsonPrimitive.Parse, "true");
            Test(JsonPrimitive.Parse, "false");
            Test(JsonPrimitive.Parse, "null");
            Test(JsonPrimitive.Parse, "3.14");
            Test(JsonPrimitive.Parse, "-3.14");
        }

        public static void Test(Func<string, IJsonValue> parser, string input)
        {
            var json = parser(input);
            Console.WriteLine(json);

            parser(json.ToString(JsonFormatStyle.Prettify));
            parser(json.ToString(JsonFormatStyle.Minify));
        }

        public static void Main(string[] args)
        {
            ParseTest();
            Console.WriteLine("====================");

            var str = @"{   ""datetime"": ""2022-06-01T13:40:00"", ""code"":
{""python"":               ""print(\""good\"")""
,""rust""
:

""print!(\""goodn\"");""}
, ""data"": [false, 1, ""    2"",
3, 4]}";

            Console.WriteLine(JsonObject.Parse(str).ToString(JsonFormatStyle.Prettify));
            Console.WriteLine(JsonObject.Parse(str).ToString(JsonFormatStyle.Minify));

        }

    }

}
