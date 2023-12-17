using System;

namespace grate.Infrastructure;

public record GrateConsoleColor
{
    public string AnsiColorCode { get; }
    private ConsoleColor Fallback { get; }

    private GrateConsoleColor(string ansiColorCode, ConsoleColor fallback)
    {
        AnsiColorCode = ansiColorCode;
        Fallback = fallback;
    }

    public static class Foreground
    {
        private static class AnsiColors
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
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
            // ReSharper restore MemberHidesStaticFromOuterClass
        }

        public static readonly GrateConsoleColor Default = new(AnsiColors.Default, ConsoleColor.White);
        public static readonly GrateConsoleColor Info = Rgb(23, 162, 184);
        public static readonly GrateConsoleColor Black = new(AnsiColors.Black, ConsoleColor.Black);
        public static readonly GrateConsoleColor DarkRed = new(AnsiColors.DarkRed, ConsoleColor.DarkRed);
        public static readonly GrateConsoleColor DarkGreen = new(AnsiColors.DarkGreen, ConsoleColor.DarkGreen);
        public static readonly GrateConsoleColor DarkYellow = new(AnsiColors.DarkYellow, ConsoleColor.DarkYellow);
        public static readonly GrateConsoleColor DarkBlue = new(AnsiColors.DarkBlue, ConsoleColor.DarkBlue);
        public static readonly GrateConsoleColor DarkMagenta = new(AnsiColors.DarkMagenta, ConsoleColor.DarkMagenta);
        public static readonly GrateConsoleColor DarkCyan = new(AnsiColors.DarkCyan, ConsoleColor.DarkCyan);
        public static readonly GrateConsoleColor Gray = new(AnsiColors.Gray, ConsoleColor.Gray);
        public static readonly GrateConsoleColor DarkGray = Rgb(192, 192, 192);
        public static readonly GrateConsoleColor Red = new(AnsiColors.Red, ConsoleColor.Red);
        public static readonly GrateConsoleColor Green = new(AnsiColors.Green, ConsoleColor.Green);
        public static readonly GrateConsoleColor Yellow = new(AnsiColors.Yellow, ConsoleColor.Yellow);
        public static readonly GrateConsoleColor Blue = new(AnsiColors.Blue, ConsoleColor.Blue);
        public static readonly GrateConsoleColor Magenta = new(AnsiColors.Magenta, ConsoleColor.Magenta);
        public static readonly GrateConsoleColor Cyan = new(AnsiColors.Cyan, ConsoleColor.Cyan);
        public static readonly GrateConsoleColor White = new(AnsiColors.White, ConsoleColor.White);

        private static GrateConsoleColor Rgb(int r, int g, int b) => new(AnsiColors.Rgb(r, g, b), ConsoleColor.Gray);

    }

    public static class Background
    {
        private static class AnsiColors
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
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
            // ReSharper restore MemberHidesStaticFromOuterClass
        }

        public static readonly GrateConsoleColor Default = new(AnsiColors.Default, ConsoleColor.Black);
        public static readonly GrateConsoleColor Info = Rgb(23, 162, 184);
        public static readonly GrateConsoleColor Black = new(AnsiColors.Black, ConsoleColor.Black);
        public static readonly GrateConsoleColor DarkRed = new(AnsiColors.DarkRed, ConsoleColor.DarkRed);
        public static readonly GrateConsoleColor DarkGreen = new(AnsiColors.DarkGreen, ConsoleColor.DarkGreen);
        public static readonly GrateConsoleColor DarkYellow = new(AnsiColors.DarkYellow, ConsoleColor.DarkYellow);
        public static readonly GrateConsoleColor DarkBlue = new(AnsiColors.DarkBlue, ConsoleColor.DarkBlue);
        public static readonly GrateConsoleColor DarkMagenta = new(AnsiColors.DarkMagenta, ConsoleColor.DarkMagenta);
        public static readonly GrateConsoleColor DarkCyan = new(AnsiColors.DarkCyan, ConsoleColor.DarkCyan);
        public static readonly GrateConsoleColor Gray = new(AnsiColors.Gray, ConsoleColor.Gray);

        private static GrateConsoleColor Rgb(int r, int g, int b) => new(AnsiColors.Rgb(r, g, b), ConsoleColor.Gray);
    }

}
