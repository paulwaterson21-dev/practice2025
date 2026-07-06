using System;
using System.IO;
using System.Reflection;

namespace CommandRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "task08.dll");

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Файл библиотеки не найден по пути: {dllPath}");
                return;
            }

            Assembly assembly =Assembly.LoadFrom(dllPath);

            Type sizeCommandType = assembly.GetType("task08.DirectorySizeCommand");
            Type findCommandType = assembly.GetType("task08.FindFilesCommand");

            if (sizeCommandType != null)
            {
                object sizeCommand = Activator.CreateInstance(sizeCommandType, AppDomain.CurrentDomain.BaseDirectory);
                MethodInfo executeMethod = sizeCommandType.GetMethod("Execute");
                executeMethod?.Invoke(sizeCommand, null);
            }

            if (findCommandType!=null)
            {
                object findCommand = Activator.CreateInstance(findCommandType, AppDomain.CurrentDomain.BaseDirectory, "*.dll");
                MethodInfo executeMethod= findCommandType.GetMethod("Execute");
                executeMethod?.Invoke(findCommand, null);
            }
        }
    }
}