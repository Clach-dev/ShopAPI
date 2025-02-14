using Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app
    .UseAuthorization()
    .UseHttpsRedirection();
    
app
    .MapControllers();

app.Run();