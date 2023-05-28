using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
using Shardion.Ooparts.Backend;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.AddConsole();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AddContext<AppJsonSerializerContext>();
});
builder.Services.AddSingleton<IStorageBackend, MemoryStorageBackend>();

var app = builder.Build();
if (app.Environment.IsDevelopment() && app.Services.GetService<IStorageBackend>() is IStorageBackend backend) {
    app.Logger.LogInformation($"Generated testing upload batch {(await backend.StoreUploadBatch(new UploadBatch(Array.Empty<Upload>()))).ToString()}");
}

app.UseFileServer();

var todosApi = app.MapGroup("/api/v0");
todosApi.MapGet("/{id}", async Task<IResult> (Guid id) => {
    if (app.Services.GetService<IStorageBackend>() is IStorageBackend backend)
    {
        app.Logger.LogDebug($"Retrieving batch {id} from backend {backend}");
        UploadBatch? batch = await backend.RetrieveUploadBatch(id);
        if (batch != null)
        {
            if ((DateTime.UtcNow - batch.CreationTimestamp).TotalMinutes >= 30)
            {
                app.Logger.LogDebug($"Expiring batch {id} in backend {backend}");
                await backend.DestroyUploadBatch(id);
                return Results.NotFound();
            }
            else
            {
                return Results.Ok(batch);
            }
        }
        else
        {
            return Results.NotFound();
        }
    }
    else
    {
        app.Logger.LogCritical($"Failed to find backend while retrieving batch {id}");
        return Results.StatusCode(500);
    }
});
todosApi.MapDelete("/{id}", async Task<IResult> (Guid id) => {
    if (app.Services.GetService<IStorageBackend>() is IStorageBackend backend) {
        app.Logger.LogDebug($"Destroying batch {id} in backend {backend}");
        await backend.DestroyUploadBatch(id);
        return Results.Ok();
    }
    else
    {
        app.Logger.LogCritical($"Failed to find backend while destroying batch {id}");
        return Results.StatusCode(500);
    }
});
app.Run();

[JsonSerializable(typeof(Guid?))]
[JsonSerializable(typeof(UploadBatch))]
[JsonSerializable(typeof(Upload))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
