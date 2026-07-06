using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public static class RuntimeCompiler
    {
        public static ICalculator CreateCalculator(string code)
        {
            var syntaxTree=CSharpSyntaxTree.ParseText(code);

            var assemblyPath=Path.GetDirectoryName(typeof(object).Assembly.Location);

            var references=new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                "DynamicCalculatorAssembly_" + Guid.NewGuid().ToString("N"),
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    throw new InvalidOperationException("Ошибка компиляции: " + string.Join("\n", failures.Select(f => f.GetMessage())));
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                var type = assembly.GetType("task11.RuntimeCalculator");
                if (type ==null) throw new TypeLoadException("Класс task11.RuntimeCalculator не найден в скомпилированной сборке.");

                return (ICalculator)Activator.CreateInstance(type);
            }
        }
    }
}