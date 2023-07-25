using System;
using System.IO;

namespace TicTacToe
{
    static class FileHelper
    {
        public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TicTacToe");

        public static string PrefsFilePath => Path.Combine(AppDataPath, "preferences.txt");
    }
}