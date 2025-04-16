using ApiRemoteWorkClientBlockChain.Dependencies;

var builder = WebApplication.CreateBuilder(args);

//Dependencies Service Collection
builder.Services.AddConfigServiceCollection(builder.Configuration);

//Controllers
builder.Services.AddControllers();

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

app.MapControllers();

app.Run();
