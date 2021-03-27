using System;
using System.IO;

namespace moo.Infrastructure
{
    internal static class TextWriterExtensions
    {
        private static bool? _supportsAnsiColors;

        public static void WriteColoredMessage(this TextWriter textWriter, string message, MooConsoleColor background, MooConsoleColor foreground)
        {
            SetColorsIfEnabled(textWriter, background, foreground);
            textWriter.Write(message);
            ResetColorsIfEnabled(textWriter);
        }
        
        public static void WriteColoredMessageLine(this TextWriter textWriter, string? message, MooConsoleColor background, MooConsoleColor foreground)
        {
            SetColorsIfEnabled(textWriter, background, foreground);
            textWriter.WriteLine(message);
            ResetColorsIfEnabled(textWriter);
        }
        
            
        private static void SetColorsIfEnabled(TextWriter textWriter, MooConsoleColor background, MooConsoleColor foreground)
        {
            if (!DisableAnsiColors)
            {
                SetColors(textWriter, background.AnsiColorCode, foreground.AnsiColorCode);
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
            textWriter.Write(MooConsoleColor.Foreground.Default.AnsiColorCode); // reset to default foreground color
            textWriter.Write(MooConsoleColor.Background.Default.AnsiColorCode); // reset to the background color
        }

        private static void SetColors(TextWriter textWriter, string backgroundColorAnsiCode, string foregroundColorAnsiCode)
        {
            textWriter.Write(backgroundColorAnsiCode);
            textWriter.Write(foregroundColorAnsiCode);
        }

        private static bool DisableAnsiColors => !SupportsAnsiColors || Console.IsOutputRedirected;
        
        private static bool SupportsAnsiColors => _supportsAnsiColors ??= GetSupportsAnsiColors();

        private static bool GetSupportsAnsiColors()
        {
            lock (Console.Out)
            {
                var (oldPosition, _) = Console.GetCursorPosition();
                SetColors(Console.Out, MooConsoleColor.Background.Gray.AnsiColorCode, MooConsoleColor.Foreground.Blue.AnsiColorCode);
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
    }
}