using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Shardion.Ooparts;
using Shardion.Ooparts.Storage;
using Shardion.Ooparts.Validation;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.AddConsole();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AddContext<AppJsonSerializerContext>();
});
builder.Services.AddSingleton<IStorageLayer, MemoryStorageLayer>();
builder.Services.AddSingleton<IValidationLayer, BasicValidationLayer>();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment() && app.Services.GetService<IStorageLayer>() is IStorageLayer backend) {
    app.Logger.LogInformation($"Generated testing upload batch {(await backend.StoreUploadBatch(new UploadBatch(Array.Empty<Upload>()))).ToString()}");
}

app.UseFileServer();

RouteGroupBuilder oopartsApi = app.MapGroup("/api/v0");
oopartsApi.MapPost("/", async Task<IResult> (IFormFileCollection files, IValidationLayer validation, IStorageLayer storage) => {
    app.Logger.LogDebug($"Storing batch into backend {storage}");
    ConcurrentBag<Upload> uploads = new();
    await Parallel.ForEachAsync<IFormFile>(files, async (IFormFile file, CancellationToken ct) => {
        uploads.Add(new Upload(file.FileName, file.OpenReadStream()));
    });
    UploadBatch? validatedBatch = await validation.ValidateUploadBatch(new UploadBatch(uploads.ToArray()));
    if (validatedBatch == null)
    {
        return Results.StatusCode(500);
    }
    else
    {
        Guid? storedBatchGuid = await storage.StoreUploadBatch(validatedBatch);
        if (storedBatchGuid == null)
        {
            return Results.StatusCode(500);
        }
        else
        {
            return Results.Ok(storedBatchGuid);
        }
    }
});
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
