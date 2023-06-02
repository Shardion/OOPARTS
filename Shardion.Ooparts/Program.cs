using System.Threading.Tasks;
using System.Threading;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Shardion.Ooparts;
using Shardion.Ooparts.Storage;
using Shardion.Ooparts.Validation;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
{
    ApplicationName = "OOPARTS",
    Args = args,
    ContentRootPath = "/var/empty/",
    WebRootPath = "/var/empty/",
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});


builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 1024;
});

builder.Services.AddSingleton<IStorageLayer, MemoryStorageLayer>();
builder.Services.AddSingleton<IValidationLayer, BasicValidationLayer>();

WebApplication app = builder.Build();

ManifestEmbeddedFileProvider efp = new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "wwwroot");

app.UseFileServer();
app.UseFileServer(new FileServerOptions
{
    FileProvider = efp
});

if (app.Environment.IsDevelopment() && app.Services.GetService<IStorageLayer>() is IStorageLayer backend)
{
    app.Logger.LogInformation($"Generated testing upload batch {(await backend.StoreUploadBatch(new UploadBatch(Array.Empty<IUpload>()))).ToString()}");
}

RouteGroupBuilder oopartsApi = app.MapGroup("/api/v0");
oopartsApi.MapPost("/", async Task<IResult> (IFormFileCollection files, IValidationLayer validation, IStorageLayer storage) =>
{
    app.Logger.LogDebug($"Storing batch into backend {storage}");
    List<MemoryUpload> uploads = new();
    foreach (IFormFile file in files)
    {
        uploads.Add(await CreateUploadFromFile(file));
    }
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
oopartsApi.MapGet("/{id}", async Task<IResult> (Guid id, IValidationLayer validation, IStorageLayer storage) =>
{
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
oopartsApi.MapGet("/{id}/archive", async Task<IResult> (Guid id, IValidationLayer validation, IStorageLayer storage) =>
{
    app.Logger.LogDebug($"Retrieving batch {id} from backend {storage}");
    UploadBatch? batch = await storage.RetrieveUploadBatch(id);
    if (batch != null)
    {
        foreach (IUpload upload in batch.Uploads)
        {
            // FIXME: This is hardcoded to return a jpg
            // I don't think you'd use OOPARTS to upload a 500mb jpeg
            return Results.Stream(upload.OpenDataStream(), "image/jpeg", "mass extinction event.jpg", null, EntityTagHeaderValue.Any, false);
        }
        return Results.StatusCode(500);
    }
    else
    {
        return Results.NotFound();
    }
});
oopartsApi.MapDelete("/{id}", async Task<IResult> (Guid id, IValidationLayer validation, IStorageLayer storage) =>
{
    app.Logger.LogDebug($"Destroying batch {id} in backend {storage}");
    await storage.DestroyUploadBatch(id);
    return Results.Ok();
});
app.Run();

static async Task<MemoryUpload> CreateUploadFromFile(IFormFile file)
{
    byte[] fileData = new byte[file.Length];
    using (Stream fileStream = file.OpenReadStream())
    {
        // FIXME: support for files >2GB
        // This unsafe cast to int and IUpload specifying int instead of long
        // are the only barriers to this, but Stream.ReadAsync does not support
        // `long` lengths, so we'd need to do some weird logic involving
        // calling ReadAsync over and over until we have *all* of the data
        await fileStream.ReadAsync(fileData, 0, (int)file.Length);
        return new MemoryUpload(file.FileName, fileData);
    }
}

[JsonSerializable(typeof(Guid?))]
[JsonSerializable(typeof(UploadBatch))]
[JsonSerializable(typeof(MemoryUpload))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
