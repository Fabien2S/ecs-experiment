// See https://aka.ms/new-console-template for more information

using EcsExp.ECS;
using EcsExp.ECS.Entities;
using EcsExp.Logging;
using EcsExp.Sample;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

LogManager.Factory = LoggerFactory.Create(builder =>
{
    builder
        .ClearProviders()
        .SetMinimumLevel(LogLevel.Debug)
        .AddSimpleConsole(options =>
        {
            // see https://no-color.org/
            options.ColorBehavior = Environment.GetEnvironmentVariable("NO_COLOR") != null
                ? LoggerColorBehavior.Disabled
                : LoggerColorBehavior.Default;
            options.SingleLine = false;

            options.IncludeScopes = true;
            options.TimestampFormat = "hh:mm:ss ";
            options.UseUtcTimestamp = false;
        });
});
var logger = LogManager.Create<Program>();

unsafe
{
    logger.LogInformation("Size of {Entity} is {Size}", nameof(EcsEntity), sizeof(EcsEntity));
}

var world = new EcsWorldBuilder()
    .AddArchetype<CubeArchetype>()
    .AddSystem<RotateTransformSystem>()
    .Build();

var entity = world.CreateEntity<CubeArchetype>();