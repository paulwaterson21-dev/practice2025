using System;
using System.IO;


namespace task08
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _directoryPath;

        public DirectorySizeCommand(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public void Execute()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Console.WriteLine($"Каталог {_directoryPath} не существует.");
                return;
            }

            long totalSize = 0;
            var files = Directory.GetFiles(_directoryPath, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                totalSize += new FileInfo(file).Length;
            }

            Console.WriteLine($"Размер каталога {_directoryPath}: {totalSize} байт");
        }
    }
}