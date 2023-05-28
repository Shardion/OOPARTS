using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
using Shardion.Ooparts;
using Shardion.Ooparts.Storage;
using Shardion.Ooparts.Validation;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.AddConsole();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AddContext<AppJsonSerializerContext>();
});
builder.Services.AddSingleton<IStorageLayer, MemoryStorageLayer>();
builder.Services.AddSingleton<IValidationLayer, BasicValidationLayer>();

var app = builder.Build();
if (app.Environment.IsDevelopment() && app.Services.GetService<IStorageLayer>() is IStorageLayer backend) {
    app.Logger.LogInformation($"Generated testing upload batch {(await backend.StoreUploadBatch(new UploadBatch(Array.Empty<Upload>()))).ToString()}");
}

app.UseFileServer();

RouteGroupBuilder oopartsApi = app.MapGroup("/api/v0");
oopartsApi.MapGet("/{id}", async Task<IResult> (Guid id, IValidationLayer validation, IStorageLayer storage) => {
    app.Logger.LogDebug($"Retrieving batch {id} from backend {storage}");
    UploadBatch? batch = await validation.ValidateUploadBatch(await storage.RetrieveUploadBatch(id));
    if (batch != null)
    {
        return Results.Ok(batch);
    }
    else
    {
        return Results.NotFound();
    }
});
oopartsApi.MapDelete("/{id}", async Task<IResult> (Guid id, IValidationLayer validation, IStorageLayer storage) => {
    app.Logger.LogDebug($"Destroying batch {id} in backend {storage}");
    await storage.DestroyUploadBatch(id);
    return Results.Ok();
});
app.Run();

[JsonSerializable(typeof(Guid?))]
[JsonSerializable(typeof(UploadBatch))]
[JsonSerializable(typeof(Upload))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
