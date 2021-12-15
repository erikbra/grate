using System;

namespace grate.Infrastructure;

public record GrateConsoleColor
{
    public string AnsiColorCode { get; }
    public ConsoleColor Fallback { get; }

    private GrateConsoleColor(string ansiColorCode, ConsoleColor fallback)
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
            
        public static readonly GrateConsoleColor Default = new GrateConsoleColor(AnsiColors.Default, ConsoleColor.White); 
        public static readonly GrateConsoleColor Info = Rgb(23, 162, 184);
        public static readonly GrateConsoleColor Black = new GrateConsoleColor(AnsiColors.Black, ConsoleColor.Black);
        public static readonly GrateConsoleColor DarkRed = new GrateConsoleColor(AnsiColors.DarkRed, ConsoleColor.DarkRed);
        public static readonly GrateConsoleColor DarkGreen = new GrateConsoleColor(AnsiColors.DarkGreen, ConsoleColor.DarkGreen);
        public static readonly GrateConsoleColor DarkYellow = new GrateConsoleColor(AnsiColors.DarkYellow, ConsoleColor.DarkYellow);
        public static readonly GrateConsoleColor DarkBlue = new GrateConsoleColor(AnsiColors.DarkBlue, ConsoleColor.DarkBlue);
        public static readonly GrateConsoleColor DarkMagenta = new GrateConsoleColor(AnsiColors.DarkMagenta, ConsoleColor.DarkMagenta);
        public static readonly GrateConsoleColor DarkCyan = new GrateConsoleColor(AnsiColors.DarkCyan, ConsoleColor.DarkCyan);
        public static readonly GrateConsoleColor Gray = new GrateConsoleColor(AnsiColors.Gray, ConsoleColor.Gray);
        public static readonly GrateConsoleColor DarkGray = Rgb(192,192,192);
        public static readonly GrateConsoleColor Red = new GrateConsoleColor(AnsiColors.Red, ConsoleColor.Red);
        public static readonly GrateConsoleColor Green = new GrateConsoleColor(AnsiColors.Green, ConsoleColor.Green);
        public static readonly GrateConsoleColor Yellow = new GrateConsoleColor(AnsiColors.Yellow, ConsoleColor.Yellow);
        public static readonly GrateConsoleColor Blue = new GrateConsoleColor(AnsiColors.Blue, ConsoleColor.Blue);
        public static readonly GrateConsoleColor Magenta = new GrateConsoleColor(AnsiColors.Magenta, ConsoleColor.Magenta);
        public static readonly GrateConsoleColor Cyan = new GrateConsoleColor(AnsiColors.Cyan, ConsoleColor.Cyan);
        public static readonly GrateConsoleColor White = new GrateConsoleColor(AnsiColors.White, ConsoleColor.White);
           
        public static GrateConsoleColor Rgb(int r, int g, int b) => new GrateConsoleColor(AnsiColors.Rgb(r, g, b), ConsoleColor.Gray);

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
            
        public static readonly GrateConsoleColor Default = new GrateConsoleColor(AnsiColors.Default, ConsoleColor.Black); 
        public static readonly GrateConsoleColor Info = Rgb(23, 162, 184);
        public static readonly GrateConsoleColor Black = new GrateConsoleColor(AnsiColors.Black, ConsoleColor.Black);
        public static readonly GrateConsoleColor DarkRed = new GrateConsoleColor(AnsiColors.DarkRed, ConsoleColor.DarkRed);
        public static readonly GrateConsoleColor DarkGreen = new GrateConsoleColor(AnsiColors.DarkGreen, ConsoleColor.DarkGreen);
        public static readonly GrateConsoleColor DarkYellow = new GrateConsoleColor(AnsiColors.DarkYellow, ConsoleColor.DarkYellow);
        public static readonly GrateConsoleColor DarkBlue = new GrateConsoleColor(AnsiColors.DarkBlue, ConsoleColor.DarkBlue);
        public static readonly GrateConsoleColor DarkMagenta = new GrateConsoleColor(AnsiColors.DarkMagenta, ConsoleColor.DarkMagenta);
        public static readonly GrateConsoleColor DarkCyan = new GrateConsoleColor(AnsiColors.DarkCyan, ConsoleColor.DarkCyan);
        public static readonly GrateConsoleColor Gray = new GrateConsoleColor(AnsiColors.Gray, ConsoleColor.Gray);
            
        public static GrateConsoleColor Rgb(int r, int g, int b) => new GrateConsoleColor(AnsiColors.Rgb(r, g, b), ConsoleColor.Gray);
    }

}