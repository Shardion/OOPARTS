using Shardion.Ooparts.Backend;
using System.Text.Json.Serialization;
using System;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.AddConsole();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AddContext<AppJsonSerializerContext>();
});
builder.Services.AddSingleton<IStorageBackend, MemoryStorageBackend>();

var app = builder.Build();
app.UseFileServer();

var todosApi = app.MapGroup("/api/v0/");
todosApi.MapGet("/retrieve/{id}", (Guid id) => {
        if (app.Services.GetService(typeof(IStorageBackend)) is IStorageBackend backend) {
            return backend.RetrieveUploadBatch(id);
        }
        return null;
    });
todosApi.MapGet("/destroy/{id}", (Guid id) => {
        if (app.Services.GetService(typeof(IStorageBackend)) is IStorageBackend backend) {
            backend.DestroyUploadBatch(id);
        }
    });
app.Run();

[JsonSerializable(typeof(Upload))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
