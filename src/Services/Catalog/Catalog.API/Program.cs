var builder = WebApplication.CreateBuilder(args);

//Add Service to the container
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.
}).UseLightweightSessions();
var app = builder.Build();

//Configure the HTTP request pipeline
app.MapCarter();
app.Run();
