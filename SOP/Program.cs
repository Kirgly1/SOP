using MongoDB.Driver;
using RabbitMQ.Client;
using SOP.Controllers;
using SOP.GraphQL;
using SOP.Services;
using SOP.Mongo;
using SOP.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Wagon API",
        Version = "v1"
    });
});
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    return new ConnectionFactory() { HostName = "localhost", Port = 5672 };
});
builder.Services.AddSingleton<RabbitMqConsumer>();

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection(nameof(MongoDBSettings)));
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDBSettings:ConnectionString")));
builder.Services.AddScoped<WagonService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapGraphQL();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
