using System;
using System.IO;
using System.Reflection;

namespace task09
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ошибка: укажите путь к файлу .dll в параметрах командной строки");
                return;
            }

            string assemblyPath = args[0];

            if (!File.Exists(assemblyPath))
            {
                Console.WriteLine($"ошибка: файл не найден по пути '{assemblyPath}'");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                Console.WriteLine($"АНАЛИЗ СБОРКИ: {assembly.GetName().Name}");

                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (!type.IsClass) continue;

                    Console.WriteLine($"Класс: {type.FullName}");

                    var attributes = type.GetCustomAttributes(false);
                    if (attributes.Length > 0)
                    {
                        Console.WriteLine("  Атрибуты класса:");
                        foreach (var attr in attributes)
                        {
                            Console.WriteLine($"    - [{attr.GetType().Name}]");
                        }
                    }

                    ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    Console.WriteLine("  Конструкторы:");
                    foreach (var ctor in constructors)
                    {
                        Console.Write($"    - {type.Name}(");
                        ParameterInfo[] parameters =ctor.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                            if (i < parameters.Length - 1) Console.Write(", ");
                        }
                        Console.WriteLine(")");
                    }

                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    Console.WriteLine("  Методы:");
                    foreach (var method in methods)
                    {
                        Console.Write($"    - {method.ReturnType.Name} {method.Name}(");
                        ParameterInfo[] parameters = method.GetParameters();
                        for (int i = 0;i < parameters.Length;i++)
                        {
                            Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                            if (i < parameters.Length - 1) Console.Write(", ");
                        }
                        Console.WriteLine(")");

                        var methodAttrs =method.GetCustomAttributes(false);
                        if (methodAttrs.Length > 0)
                        {
                            foreach (var attr in methodAttrs)
                            {
                                Console.WriteLine($"        * Атрибут метода: [{attr.GetType().Name}]");
                            }
                        }
                    }

                    Console.WriteLine(new string('-', 50));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при анализе сборки: {ex.Message}");
            }
        }
    }
}