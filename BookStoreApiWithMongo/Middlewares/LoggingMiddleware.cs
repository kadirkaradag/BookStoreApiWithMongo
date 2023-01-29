using MongoDB.Bson;
using MongoDB.Driver;

namespace BookStoreApiWithMongo.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly IMongoDatabase _database;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IMongoDatabase database)
        {
            _next = next;
            _logger = logger;
            _database = database;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                var request = context.Request;
                var httpStatus = request.HttpContext.Response.StatusCode;
                _logger.LogInformation($"{request.Method} {request.Path}");
                var collection = _database.GetCollection<BsonDocument>("Requests");
                await collection.InsertOneAsync(new BsonDocument { { "Method", request.Method }, { "Path", request.Path.Value }, { "Code", httpStatus } });
            }            
        }
    }

}
