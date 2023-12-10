namespace grate.Infrastructure;

internal static class TextWriterExtensions
{
    private static bool? _supportsAnsiColors;

    public static void WriteColoredMessage(this TextWriter textWriter, string message, GrateConsoleColor foreground)
    {
        SetColorsIfEnabled(textWriter, foreground);
        textWriter.Write(message);
        ResetColorsIfEnabled(textWriter);
    }

    public static void WriteColoredMessageLine(this TextWriter textWriter, string? message, GrateConsoleColor foreground)
    {
        SetColorsIfEnabled(textWriter, foreground);
        textWriter.WriteLine(message);
        ResetColorsIfEnabled(textWriter);
    }


    private static void SetColorsIfEnabled(TextWriter textWriter, GrateConsoleColor foreground)
    {
        if (!DisableAnsiColors)
        {
            SetColors(textWriter, foreground.AnsiColorCode);
        }
    }


    private static void ResetColorsIfEnabled(TextWriter textWriter)
    {
        if (!DisableAnsiColors)
        {
            ResetColors(textWriter);
        }
    }

    private static void ResetColors(TextWriter textWriter)
    {
        textWriter.Write(GrateConsoleColor.Foreground.Default.AnsiColorCode); // reset to default foreground color
    }

    private static void SetColors(TextWriter textWriter, string foregroundColorAnsiCode)
    {
        textWriter.Write(foregroundColorAnsiCode);
    }

    private static bool DisableAnsiColors => !SupportsAnsiColors || Console.IsOutputRedirected;

    private static bool SupportsAnsiColors => _supportsAnsiColors ??= GetSupportsAnsiColors();

    private static bool GetSupportsAnsiColors()
    {
        try
        // Calling Console.GetCursorPosition() sometimes fails if the console has not been written to yet
        {
            lock (Console.Out)
            {
                var (oldPosition, _) = Console.GetCursorPosition();
                SetColors(Console.Out, GrateConsoleColor.Foreground.Blue.AnsiColorCode);
                var (currentPosition, yPos) = Console.GetCursorPosition();

                ResetColors(Console.Out);

                if (currentPosition != oldPosition)
                {
                    Console.SetCursorPosition(oldPosition, yPos);
                    Console.Out.Write("                                                                                ");
                    Console.SetCursorPosition(oldPosition, yPos);
                    return false;
                }

                return true;
            }
        }
        catch (Exception)
        {
            return true;
        }
    }
}
