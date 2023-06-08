using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MongoClient>(_=> new MongoClient());
builder.Services.AddSingleton<IMongoDatabase>(
    provider => provider.GetRequiredService<MongoClient>().GetDatabase("building-apis")
);
builder.Services.AddSingleton<IMongoCollection<Company>>(
    provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Company>("companies")
);
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new ObjectIdJsonConverter());
});
var app = builder.Build();


app.MapGet("/", () => "Hello World!");

app.MapGet("/companies",GetCompanies);

app.MapPost("/companies", AddCompanies);

app.MapGet("/companies/{companyId}/offices", GetOffices);

app.Run();

static async Task<IResult> GetCompanies(IMongoCollection<Company> collection){
    return TypedResults.Ok(await collection.Find(Builders<Company>.Filter.Empty).ToListAsync());
};

static async Task<IResult> AddCompanies(IMongoCollection<Company> collection, Company company) {
   company = company with{ Id = ObjectId.Empty };
    await collection.InsertOneAsync(company);
    return TypedResults.Ok(company); 
};

static async Task<IResult> GetOffices(IMongoCollection<Company> collection, ObjectId companyId) {
    var offices = await collection.Find(
        Builders<Company>.Filter.Eq(x => x.Id,companyId))
        .Project(x => x.Offices)
        .FirstOrDefaultAsync();
        return TypedResults.Ok(offices);
    
};

public class ObjectIdJsonConverter : JsonConverter<ObjectId>
{
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new (reader.GetString());
    
    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
};
public record Company(ObjectId Id, string Name, IReadOnlyCollection<Office> Offices);
public record Office(string Id, Address Address);
public record Address(string Line1, string Line2, string PostalCode, string Country);