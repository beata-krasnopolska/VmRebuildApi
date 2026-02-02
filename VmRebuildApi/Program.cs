using VmRebuildApi.Jobs;
using VmRebuildApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IJobStore, InMemoryJobStore>();
builder.Services.AddSingleton<PowerShellRunner>();
builder.Services.AddSingleton<RebuildJobWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (ctx, next) =>
{
    var cfgKey = app.Configuration["ApiKey"];
    if (!string.IsNullOrWhiteSpace(cfgKey))
    {
        if (!ctx.Request.Headers.TryGetValue("X-API-KEY", out var sent) || sent != cfgKey)
        {
            ctx.Response.StatusCode = 401;
            await ctx.Response.WriteAsync("Missing or invalid API key.");
            return;
        }
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
