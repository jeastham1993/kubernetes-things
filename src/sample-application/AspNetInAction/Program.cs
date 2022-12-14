using System.Text.Json;
using Amazon.Lambda.Serialization.SystemTextJson;
using AspNetInAction;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// If running in Kubernetes configmap is mounted through a volume to the /config folder
if (File.Exists("./appsettings.k8s.json"))
{
    builder.Configuration.AddJsonFile("./appsettings.k8s.json");
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AddContext<ApiSerializerContext>();
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi, options =>
{
    options.Serializer = new SourceGeneratorLambdaJsonSerializer<ApiSerializerContext>();
});

builder.Services.AddProblemDetails();
builder.Services.AddLogging();

WebApplication app = builder.Build();
app.UseStatusCodePages();

var config = app.Services.GetRequiredService<IConfiguration>();

app.MapGet("/health", () => "I'm healthy!!!");

app.MapGet("/pod-details", () => new {
    podName = Environment.GetEnvironmentVariable("POD_NAME"),
    podNamespace = Environment.GetEnvironmentVariable("POD_NAMESPACE"),
    podIp = Environment.GetEnvironmentVariable("POD_IP")
});

app.MapGet("/config", () => config["ApiResponseValue"]);
app.MapGet("/secret-test", () => Environment.GetEnvironmentVariable("DB_PASSWORD"));

// Create a route group on the API, adding an endpoint filter to all routes.
var fruitApiRouteBuilder = app.MapGroup("/fruit")
    .AddEndpointFilter<LoggingFilter>();

fruitApiRouteBuilder.MapGet("/", () => Handlers.GetAllFruits());

// Create an additional route group builder that implements validation of the id parameter
var fruitApiWithValidation = fruitApiRouteBuilder
    .AddEndpointFilter<IdValidationFilter>();

// Use that to add additional routes that will now all be validated
fruitApiWithValidation.MapGet("/{id}", Handlers.GetFruit);

fruitApiWithValidation.MapPost("/{id}", Handlers.AddFruit);

fruitApiWithValidation.MapPut("/{id}", Handlers.ReplaceFruit);

fruitApiWithValidation.MapDelete("/{id}", Handlers.DeleteFruit);

app.Run();

public record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
};

class IdValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var id = context.HttpContext.Request.RouteValues["id"].ToString();
        
        if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    {"id", new[] {"Invalid format. Id must start with 'f'"}}
                });
        }

        return await next(context);
    }
}

class LoggingFilter : IEndpointFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        this._logger.LogInformation("Executing filter...");
        var result = await next(context);
        this._logger.LogInformation($"Handler result: {result}");
        return result;
    }
}

class Handlers
{
    public static IResult GetAllFruits()
    {
        return Results.Ok(Fruit.All);
    }

    public static IResult GetFruit(string id)
    {
        return Fruit.All.TryGetValue(id, out var fruit)
            ? Results.Ok(fruit)
            : Results.Problem(statusCode: 404, detail: "This fruit does not exist", title: "Missing Fruit");
    }

    public static async Task ReplaceFruit(string id, Fruit fruit)
    {
        Fruit.All[id] = fruit;
    }

    public static async Task AddFruit(string id, Fruit fruit)
    {
        Fruit.All.Add(id, fruit);
    }

    public static void DeleteFruit(string id)
    {
        Fruit.All.Remove(id);
    }
}