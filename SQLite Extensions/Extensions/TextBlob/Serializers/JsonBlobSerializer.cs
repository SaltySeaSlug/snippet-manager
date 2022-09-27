using System;
using System.Text.Json;

namespace SQLiteNetExtensions.Extensions.TextBlob.Serializers
{
    public class JsonBlobSerializer : ITextBlobSerializer
    {
        public string Serialize(object element)
        {
            return JsonSerializer.Serialize(element);
        }

        public object Deserialize(string text, Type type)
        {
            return JsonSerializer.Deserialize(text, type);
        }
    }
}