using System;

namespace Json
{
    public class Program
    {
        public static void ParseTest()
        {
            Console.WriteLine(JsonObject.Parse("{}"));
            Console.WriteLine(JsonObject.Parse("{\"A\": null}"));
            Console.WriteLine(JsonObject.Parse("{\"A\": null, \"B\": 1}"));
            Console.WriteLine(JsonObject.Parse("{\"A\": null, \"B\": 1, \"C\": \"123\"}"));
            Console.WriteLine(JsonObject.Parse("{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}}"));
            Console.WriteLine(JsonObject.Parse("{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}, \"E\": { \"F\": true}, \"G\": []}"));
            Console.WriteLine(JsonObject.Parse("{\"A\": null, \"B\": 1, \"C\": \"123\", \"D\": {}, \"E\": { \"F\": true}, \"G\": [], \"H\": [1 ,2,3]}"));


            Console.WriteLine(JsonArray.Parse("[ {}, {\"A\": [ null]}]"));

            Console.WriteLine(JsonPrimitive.Parse("\"A\\\"S\tD\""));

            Console.WriteLine(JsonPrimitive.Parse("\"\\u0061\""));
            Console.WriteLine(JsonPrimitive.Parse("true"));
            Console.WriteLine(JsonPrimitive.Parse("false"));
            Console.WriteLine(JsonPrimitive.Parse("null"));
            Console.WriteLine(JsonPrimitive.Parse("3.14"));
            Console.WriteLine(JsonPrimitive.Parse("-3.14"));
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
