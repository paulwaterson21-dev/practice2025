using System;
using System.Reflection;

namespace task07
{
    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            if (type == null) return;

            Console.WriteLine($"Анализ типа:{type.Name}");
            Console.WriteLine("------------------------------------");

            var classDisplayName = type.GetCustomAttribute<DisplayNameAttribute>();
            if (classDisplayName !=null)
            {
                Console.WriteLine($"Отображаемое имя класса:{classDisplayName.DisplayName}");
            }

            var versionAttr= type.GetCustomAttribute<VersionAttribute>();
            if (versionAttr != null)
            {
                Console.WriteLine($"Версия класса: {versionAttr.Major}.{versionAttr.Minor}");
            }

            Console.WriteLine("\nСвойства класса:");
            foreach (var prop in type.GetProperties())
            {
                var propDisplay = prop.GetCustomAttribute<DisplayNameAttribute>();
                string displayNameInfo = propDisplay !=null ? $" ({propDisplay.DisplayName})" : "";
                Console.WriteLine($"  - Свойство: {prop.PropertyType.Name} {prop.Name}{displayNameInfo}");
            }

            Console.WriteLine("\nМетоды класса:");
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var methodDisplay = method.GetCustomAttribute<DisplayNameAttribute>();
                string displayNameInfo = methodDisplay != null ? $" ({methodDisplay.DisplayName})" : "";
                Console.WriteLine($"  - Метод: {method.Name}{displayNameInfo}");
            }
        }
    }
}