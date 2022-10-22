using MongoDB.Bson;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;

namespace Demo.Database;

internal static class MongoDBExtensions
{
    public static void AddLogger(this ClusterBuilder builder, ILogger logger)
    {
        builder.Subscribe<CommandStartedEvent>(e =>
        {
            if(IgnoreLogger(e.CommandName))
            {
                return;
            }
            logger.LogInformation($"[DATABASE][QUERY][{e.CommandName}][{e.RequestId}] {e.Command.ToJson()}");
        });

        builder.Subscribe<CommandSucceededEvent>(e =>
        {
            if(IgnoreLogger(e.CommandName))
            {
                return;
            }
            logger.LogInformation($"[DATABASE][RESULT][{e.CommandName}][{e.RequestId}] {e.Reply.Values.ToJson()}");
        });

        builder.Subscribe<CommandFailedEvent>(e =>
        {
            if(IgnoreLogger(e.CommandName))
            {
                return;
            }
            logger.LogInformation($"[DATABASE][FAILURE][{e.CommandName}][{e.RequestId}] {e.Failure.ToJson()}");
        });

        static bool IgnoreLogger(string commandName)
            => "isMaster".Equals(commandName, StringComparison.CurrentCultureIgnoreCase)
            ? true
            : false;
    }
}
