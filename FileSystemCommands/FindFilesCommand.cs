using System;
using System.IO;


namespace task08
{
    public class FindFilesCommand : ICommand
    {
        private readonly string _directoryPath;
        private readonly string _searchPattern;

        public FindFilesCommand(string directoryPath,string searchPattern)
        {
            _directoryPath = directoryPath;
            _searchPattern =searchPattern;
        }

        public void Execute()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Console.WriteLine($"Каталог {_directoryPath} несуществует.");
                return;
            }

            var files = Directory.GetFiles(_directoryPath, _searchPattern);
            Console.WriteLine($"Найденные файлы в {_directoryPath} по маске {_searchPattern}:");
            foreach (var file in files)
            {
                Console.WriteLine($"  -{Path.GetFileName(file)}");
            }
        }
    }
}