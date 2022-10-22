using Demo.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;

namespace Demo;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();


        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        services.AddSingleton<MongoUrl>(sp => {
            var connectionString = sp
                .GetRequiredService<IConfiguration>()
                .GetConnectionString("MongoDB");

            return new MongoUrl(connectionString);
        });
        services.AddSingleton<IMongoClient>(sp => {
            var mongoUrl = sp.GetService<MongoUrl>();

            var mongoClientSettings = new MongoClientSettings();
            mongoClientSettings.Server = mongoUrl.Server;
            mongoClientSettings.ClusterConfigurator = clusterBuilder
                => clusterBuilder.AddLogger(
                    sp.GetService<ILogger<IMongoClient>>()
                );

            return new MongoClient(mongoClientSettings);
        });

        services.AddScoped<IMongoDatabase>(sp => {
            var mongoUrl = sp.GetService<MongoUrl>();
            var mongoClient = sp.GetService<IMongoClient>();

            return mongoClient.GetDatabase(mongoUrl.DatabaseName);
        });

        services.AddScoped<IProductRepository, ProductRepository>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        app.UseEndpoints(endpoints
            => endpoints.MapControllers() // Mapping all controller
        );
    }
}
