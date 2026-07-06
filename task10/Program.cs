using System;
using System.Collections.Generic;
using System.Linq;

namespace task10
{

    public interface IPlugin
    {
        void Execute();
    }


    [PluginLoad("PluginA", "PluginB")]
    public class PluginA : IPlugin
    {
        public void Execute()
        {

        }
    }

    [PluginLoad("PluginB")]
    public class PluginB : IPlugin
    {
        public void Execute()
        {

        }
    }

    public class PluginManager
    {
        public void LoadAndExecutePlugins(List<IPlugin> plugins)
        {
            if (plugins == null) throw new ArgumentNullException(nameof(plugins));

            var adjacencyList = new Dictionary<string, List<string>>();
            var pluginDict = new Dictionary<string, IPlugin>();

            foreach (var plugin in plugins)
            {
                var type = plugin.GetType();
                var attr = (PluginLoadAttribute)Attribute.GetCustomAttribute(type, typeof(PluginLoadAttribute));

                if (attr == null) continue;

                string pluginName = attr.PluginName;
                string[] deps = attr.Dependencies ?? Array.Empty<string>();

                pluginDict[pluginName] = plugin;
                adjacencyList[pluginName] = new List<string>(deps);
            }

            var visited = new Dictionary<string, byte>();
            var sortedPluginNames = new List<string>();

            foreach (var name in pluginDict.Keys)
            {
                visited[name] = 0;
            }

            try
            {
                foreach (var name in pluginDict.Keys)
                {
                    if (visited[name]==0)
                    {
                        DepthFirstSearch(name, adjacencyList, visited, sortedPluginNames, pluginDict);
                    }
                }


                Console.WriteLine("--- Порядок запуска плагинов согласно графу зависимостей ---");
                foreach (var name in sortedPluginNames)
                {
                    if (pluginDict.ContainsKey(name))
                    {
                        try
                        {
                            Console.WriteLine($"Запуск плагина: {name}");
                            pluginDict[name].Execute();
                            Console.WriteLine($"Плагин {name} успешно завершил работу.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при работе плагина {name}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки плагинов: {ex.Message}");
            }
        }

        private void DepthFirstSearch(string node, Dictionary<string, List<string>> graph, Dictionary<string, byte> visited, List<string> sorted, Dictionary<string, IPlugin> pluginDict)
        {
            visited[node] = 1;

            if (graph.ContainsKey(node))
            {
                foreach (var neighbor in graph[node])
                {
                    if (!pluginDict.ContainsKey(neighbor))
                    {
                        throw new InvalidOperationException($"Ошибка: Плагин '{node}' зависит от отсутствующего плагина '{neighbor}'.");
                    }

                    if (visited[neighbor] ==1)
                    {
                        throw new InvalidOperationException($"Обнаружена циклическая зависимость между '{node}' и '{neighbor}'!");
                    }

                    if (visited[neighbor] == 0)
                    {
                        DepthFirstSearch(neighbor, graph, visited, sorted, pluginDict);
                    }
                }
            }

            visited[node] = 2;
            sorted.Add(node);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var plugins = new List<IPlugin> { new PluginA(), new PluginB() };
            var manager = new PluginManager();
            manager.LoadAndExecutePlugins(plugins);
        }
    }
}