using System;

namespace moo.Infrastructure
{
    public record MooConsoleColor
    {
        public string AnsiColorCode { get; }
        public ConsoleColor Fallback { get; }

        private MooConsoleColor(string ansiColorCode, ConsoleColor fallback)
        {
            AnsiColorCode = ansiColorCode;
            Fallback = fallback;
        }
        
        public static class Foreground
        {
            private static class AnsiColors
            {
                internal const string Default = "\x1B[39m\x1B[22m";
                internal const string Black = "\x1B[30m";
                internal const string DarkRed = "\x1B[31m";
                internal const string DarkGreen = "\x1B[32m";
                internal const string DarkYellow = "\x1B[33m";
                internal const string DarkBlue = "\x1B[34m";
                internal const string DarkMagenta = "\x1B[35m";
                internal const string DarkCyan = "\x1B[36m";
                internal const string Gray = "\x1B[37m";
                internal const string Red = "\x1B[1m\x1B[31m";
                internal const string Green = "\x1B[1m\x1B[32m";
                internal const string Yellow = "\x1B[1m\x1B[33m";
                internal const string Blue = "\x1B[1m\x1B[34m";
                internal const string Magenta = "\x1B[1m\x1B[35m";
                internal const string Cyan = "\x1B[1m\x1B[36m";
                internal const string White = "\x1B[1m\x1B[37m";
                
                internal static string Rgb(int r, int g, int b) => $"\x1b[38;2;{r};{g};{b}m";
            }
            
            public static readonly MooConsoleColor Default = new MooConsoleColor(AnsiColors.Default, ConsoleColor.White); 
            public static readonly MooConsoleColor Info = Rgb(23, 162, 184);
            public static readonly MooConsoleColor Black = new MooConsoleColor(AnsiColors.Black, ConsoleColor.Black);
            public static readonly MooConsoleColor DarkRed = new MooConsoleColor(AnsiColors.DarkRed, ConsoleColor.DarkRed);
            public static readonly MooConsoleColor DarkGreen = new MooConsoleColor(AnsiColors.DarkGreen, ConsoleColor.DarkGreen);
            public static readonly MooConsoleColor DarkYellow = new MooConsoleColor(AnsiColors.DarkYellow, ConsoleColor.DarkYellow);
            public static readonly MooConsoleColor DarkBlue = new MooConsoleColor(AnsiColors.DarkBlue, ConsoleColor.DarkBlue);
            public static readonly MooConsoleColor DarkMagenta = new MooConsoleColor(AnsiColors.DarkMagenta, ConsoleColor.DarkMagenta);
            public static readonly MooConsoleColor DarkCyan = new MooConsoleColor(AnsiColors.DarkCyan, ConsoleColor.DarkCyan);
            public static readonly MooConsoleColor Gray = new MooConsoleColor(AnsiColors.Gray, ConsoleColor.Gray);
            public static readonly MooConsoleColor Red = new MooConsoleColor(AnsiColors.Red, ConsoleColor.Red);
            public static readonly MooConsoleColor Green = new MooConsoleColor(AnsiColors.Green, ConsoleColor.Green);
            public static readonly MooConsoleColor Yellow = new MooConsoleColor(AnsiColors.Yellow, ConsoleColor.Yellow);
            public static readonly MooConsoleColor Blue = new MooConsoleColor(AnsiColors.Blue, ConsoleColor.Blue);
            public static readonly MooConsoleColor Magenta = new MooConsoleColor(AnsiColors.Magenta, ConsoleColor.Magenta);
            public static readonly MooConsoleColor Cyan = new MooConsoleColor(AnsiColors.Cyan, ConsoleColor.Cyan);
            public static readonly MooConsoleColor White = new MooConsoleColor(AnsiColors.White, ConsoleColor.White);
           
            public static MooConsoleColor Rgb(int r, int g, int b) => new MooConsoleColor(AnsiColors.Rgb(r, g, b), ConsoleColor.Gray);

        }

        public static class Background
        {
            private static class AnsiColors
            {
                internal const string Default = "\x1B[49m"; // reset to the background color

                internal const string Black = "\x1B[40m";
                internal const string DarkRed = "\x1B[41m";
                internal const string DarkGreen = "\x1B[42m";
                internal const string DarkYellow = "\x1B[43m";
                internal const string DarkBlue = "\x1B[44m";
                internal const string DarkMagenta = "\x1B[45m";
                internal const string DarkCyan = "\x1B[46m";
                internal const string Gray = "\x1B[47m";

                internal static string Rgb(int r, int g, int b) => $"\x1b[48;2;{r};{g};{b}m";
            }
            
            public static readonly MooConsoleColor Default = new MooConsoleColor(AnsiColors.Default, ConsoleColor.Black); 
            public static readonly MooConsoleColor Info = Rgb(23, 162, 184);
            public static readonly MooConsoleColor Black = new MooConsoleColor(AnsiColors.Black, ConsoleColor.Black);
            public static readonly MooConsoleColor DarkRed = new MooConsoleColor(AnsiColors.DarkRed, ConsoleColor.DarkRed);
            public static readonly MooConsoleColor DarkGreen = new MooConsoleColor(AnsiColors.DarkGreen, ConsoleColor.DarkGreen);
            public static readonly MooConsoleColor DarkYellow = new MooConsoleColor(AnsiColors.DarkYellow, ConsoleColor.DarkYellow);
            public static readonly MooConsoleColor DarkBlue = new MooConsoleColor(AnsiColors.DarkBlue, ConsoleColor.DarkBlue);
            public static readonly MooConsoleColor DarkMagenta = new MooConsoleColor(AnsiColors.DarkMagenta, ConsoleColor.DarkMagenta);
            public static readonly MooConsoleColor DarkCyan = new MooConsoleColor(AnsiColors.DarkCyan, ConsoleColor.DarkCyan);
            public static readonly MooConsoleColor Gray = new MooConsoleColor(AnsiColors.Gray, ConsoleColor.Gray);
            
            public static MooConsoleColor Rgb(int r, int g, int b) => new MooConsoleColor(AnsiColors.Rgb(r, g, b), ConsoleColor.Gray);
        }

    }
}