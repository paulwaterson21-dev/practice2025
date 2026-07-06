using System;
using System.Reflection;
using System.Text;

namespace task09
{
    public class ClassAnalyzer
    {
        public string AnalyzeAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentNullException(nameof(assemblyPath), "Путь к сборке не может быть пустым.");

            StringBuilder sb = new StringBuilder();

            try
            {

                Assembly assembly = Assembly.LoadFrom(assemblyPath);


                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    sb.AppendLine($"Класс: {type.FullName}");


                    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Instance | BindingFlags.Static;


                    var constructors = type.GetConstructors(flags);
                    sb.AppendLine("  Конструкторы:");
                    foreach (var ctor in constructors)
                    {
                        sb.AppendLine($"    - {ctor.Name}");
                    }

                    var methods = type.GetMethods(flags);
                    sb.AppendLine("  Методы:");
                    foreach (var method in methods)
                    {
                        var paramsInfo = method.GetParameters();

                        string paramString = string.Join(", ", Array.ConvertAll(paramsInfo, p => $"{p.ParameterType.Name} {p.Name}"));
                        sb.AppendLine($"    - {method.ReturnType.Name} {method.Name}({paramString})");
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Ошибка при анализе DLL: {ex.Message}");
            }

            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Рефлексивный анализатор готов к работе.");
        }
    }
}