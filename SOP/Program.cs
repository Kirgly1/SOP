using MongoDB.Driver;
using SOP.Controllers;
using SOP.GraphQL;
using SOP.Services;
using SOP.Mongo;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>();

builder.Services.AddGraphQLServer()
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
