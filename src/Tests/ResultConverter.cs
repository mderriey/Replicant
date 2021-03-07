using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VerifyTests;

class ResultConverter :
    WriteOnlyJsonConverter<Result>
{
    public override void WriteJson(
        JsonWriter writer,
        Result result,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WritePropertyName("Status");
        serializer.Serialize(writer, result.Status.ToString());
        writer.WritePropertyName("Meta");
        serializer.Serialize(writer, File.ReadAllText(result.MetaPath!));
        writer.WritePropertyName("Response");
        using var message = result.AsResponseMessage();
        serializer.Serialize(writer, message);
    }
}