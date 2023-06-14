global using MongoDB.Bson;
global using MongoDB.Driver;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using minimal_api;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(_=> new MongoClient());
builder.Services.AddSingleton(
    provider => provider.GetRequiredService<MongoClient>().GetDatabase("building-apis")
);
builder.Services.AddSingleton(
    provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Company>("companies"));
builder.Services.AddSingleton(
    provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Member>("members"));
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new ObjectIdJsonConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Company API",
        Description = "A .net minimal api with MongoDB",
        Version = "v1"
    });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Company API V1");
});


app.MapGet("/", () => "Hello World!");

app.MapGet("/companies",ApiMethods.GetCompanies);

app.MapPost("/companies", ApiMethods.AddCompanies);

app.MapPut("/companies", ApiMethods.UpdateCompany);

app.MapDelete("/companies/{companyId}", ApiMethods.DeleteCompany);

app.MapGet("/companies/{companyId}/offices", ApiMethods.GetOffices);

app.MapGet("/members", MemberMethods.GetMembers);

app.MapGet("/members/{args}", MemberMethods.GetMembersByFilter);

app.MapPost("/members", MemberMethods.AddMember);

app.Run();



public class ObjectIdJsonConverter : JsonConverter<ObjectId>
{
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new (reader.GetString());
    
    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
};
