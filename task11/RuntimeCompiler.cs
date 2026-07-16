using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace task11
{
    public class RuntimeCompiler
    {
        public static ICalculator CreateCalculator(string sourceCode)
        {
            return CompileCalculatorSource(sourceCode);
        }

        public static ICalculator CompileCalculatorSource(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
                throw new ArgumentException("Код не может быть пустым", nameof(sourceCode));

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            string assemblyName = Path.GetRandomFileName();

  
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var references = new List<MetadataReference>();

            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var errors = string.Join(Environment.NewLine, result.Diagnostics
                        .Where(d => d.Severity == DiagnosticSeverity.Error)
                        .Select(d => $"{d.Id}: {d.GetMessage()}"));
                    throw new InvalidOperationException($"Критическая ошибка компиляции: {errors}");
                }

                ms.Position = 0;
                Assembly assembly = Assembly.Load(ms.ToArray());


                Type type = assembly.GetTypes()
                    .FirstOrDefault(t => typeof(ICalculator).IsAssignableFrom(t) && !t.IsInterface);

                if (type == null)
                    throw new InvalidOperationException("Класс, реализующий ICalculator, не найден в сгенерированной сборке.");

                return (ICalculator)Activator.CreateInstance(type);
            }
        }
    }
}