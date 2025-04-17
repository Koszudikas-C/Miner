using ApiRemoteWorkClientBlockChain.Dependencies;
using LibMiddleware.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

//Dependencies Service Collection
builder.Services.AddConfigServiceCollection(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseHttpLogging();

app.Use(async (context, next) => 
{
    app.Logger.LogInformation($"Request: {context.Connection.RemotePort} " + 
    $"{context.Connection.RemoteIpAddress} {context.Request.Method} {context.Request.Path}");

    await next(context);
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseForwardedHeaders();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
