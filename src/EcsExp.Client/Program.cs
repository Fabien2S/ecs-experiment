using EcsExp.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;

// Initialize logger

namespace EcsExp.Client;

internal unsafe class Program
{
    public static int Main(string[] args)
    {
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

        // Initialize GLFW
        var glfw = Glfw.GetApi();
        if (!glfw.Init())
        {
            logger.LogCritical("Failed to initialize GLFW");
            return -1;
        }

        // Configure window
        {
            glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            glfw.WindowHint(WindowHintBool.OpenGLDebugContext, true);
            glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
            glfw.WindowHint(WindowHintInt.ContextVersionMajor, 4);
            glfw.WindowHint(WindowHintInt.ContextVersionMinor, 6);
        }

        // Create window
        const int height = 480;
        const int width = height * 16 / 9;
        var window = glfw.CreateWindow(width, height, nameof(EcsExp), null, null);
        if (window == null)
        {
            logger.LogCritical("Failed to create window");
            return -1;
        }
        
        // Configure OpenGL
        {
            glfw.SwapInterval(1);
            glfw.MakeContextCurrent(window);
        }

        // GL.GetApi();

        return 0;
    }
}