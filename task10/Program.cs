using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task10
{
    internal class Program
    {
        private static void Main(string[] args)
        {
          
            string pluginsDir = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"Сканирование директории плагинов: {pluginsDir}\n");

            if (!Directory.Exists(pluginsDir))
            {
                Console.WriteLine("Директория не найдена.");
                return;
            }

            var pluginTypes= new List<(Type Type, PluginLoadAttribute Attr)>();


            string[] dllFiles = Directory.GetFiles(pluginsDir, "*.dll");
            foreach (string dll in dllFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dll);
                    foreach (Type type in assembly.GetTypes())
                    {
                        var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                        if (attr != null && type.IsClass)
                        {
                            pluginTypes.Add((type, attr));
                        }
                    }
                }
                catch
                {
       
                }
            }

            if (pluginTypes.Count == 0)
            {
                Console.WriteLine("Плагины с атрибутом [PluginLoad] не обнаружены.");
                return;
            }

          
            try
            {
                var sortedPlugins = TopologicallySortPlugins(pluginTypes);

                Console.WriteLine("Порядок загрузки плагинов с учетом зависимостей:");
                foreach (var plugin in sortedPlugins)
                {
                    Console.WriteLine($"  -> {plugin.Attr.PluginName}");
                }
                Console.WriteLine("\nЗапуск выполнения плагинов:");
                Console.WriteLine(new string('-', 40));

            
                foreach (var plugin in sortedPlugins)
                {
                    try
                    {
                        object pluginInstance = Activator.CreateInstance(plugin.Type);
                        MethodInfo executeMethod = plugin.Type.GetMethod("Execute");

                        if (executeMethod != null)
                        {
                            Console.WriteLine($"[Запуск] {plugin.Attr.PluginName}...");
                            executeMethod.Invoke(pluginInstance, null);
                        }
                        else
                        {
                            Console.WriteLine($"[Ошибка] У плагина {plugin.Attr.PluginName} отсутствует метод Execute.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Ошибка] Не удалось запуститт плагин {plugin.Attr.PluginName}: {ex.Message}");
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[Ошибка графа зависимостей] {ex.Message}");
            }
        }

        private static List<(Type Type, PluginLoadAttribute Attr)> TopologicallySortPlugins(List<(Type Type, PluginLoadAttribute Attr)> plugins)
        {
            var result = new List<(Type Type, PluginLoadAttribute Attr)>();
            var visited = new Dictionary<string, bool>(); 
            var pluginMap = plugins.ToDictionary(p => p.Attr.PluginName, p => p);

            void Visit(string name)
            {
                if (!pluginMap.ContainsKey(name)) return;

                if (visited.TryGetValue(name, out bool isTested))
                {
                    if (!isTested)
                    {
                        throw new InvalidOperationException($"Обнаружена циклическач зависимость в плагине: {name}");
                    }
                    return;
                }

                visited[name] =false; 

                foreach (var dependency in pluginMap[name].Attr.Dependencies)
                {
                    Visit(dependency);
                }

                visited[name] = true; 
                result.Add(pluginMap[name]);
            }

            foreach (var plugin in plugins)
            {
                if (!visited.ContainsKey(plugin.Attr.PluginName))
                {
                    Visit(plugin.Attr.PluginName);
                }
            }

            return result;
        }
    }
}