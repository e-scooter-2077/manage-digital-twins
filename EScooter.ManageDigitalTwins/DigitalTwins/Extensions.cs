using Azure;
using Azure.DigitalTwins.Core;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace EScooter.ManageDigitalTwins.DigitalTwins;

public static class Extensions
{
    public static async Task<IEnumerable<T>> ListQuery<T>(this DigitalTwinsClient client, string query)
    {
        var result = client.QueryAsync<T>(query);
        var items = new List<T>();
        await foreach (var twin in result)
        {
            items.Add(twin);
        }
        return items;
    }

    public static async Task CreateTwin(
        this DigitalTwinsClient digitalTwinsClient,
        Guid id,
        string modelId,
        Action<BasicDigitalTwin> editTwin = null)
    {
        var twinData = new BasicDigitalTwin
        {
            Id = id.ToString()
        };
        twinData.Metadata.ModelId = modelId;
        editTwin?.Invoke(twinData);
        await digitalTwinsClient.CreateOrReplaceDigitalTwinAsync(twinData.Id, twinData);
    }

    public static async Task DeleteTwin(this DigitalTwinsClient digitalTwinsClient, Guid id)
    {
        await digitalTwinsClient.DeleteDigitalTwinAsync(id.ToString());
    }

    public static void AppendReplaceIfPresent<T>(this JsonPatchDocument patch, string path, T? nullableValue)
        where T : struct
    {
        if (nullableValue.HasValue)
        {
            patch.AppendReplace($"/{path}", nullableValue.Value);
        }
    }

    public static JsonElement ReadProperty(this BasicDigitalTwin twin, string propertyName) =>
        (JsonElement)twin.Contents[propertyName];
}
