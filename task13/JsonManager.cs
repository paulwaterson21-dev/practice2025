using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public static class JsonManager
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string Serialize(Student student)
        {
            return JsonSerializer.Serialize(student, Options);
        }

        public static Student Deserialize(string json)
        {
            var student = JsonSerializer.Deserialize<Student>(json, Options);
            if (student == null || string.IsNullOrEmpty(student.FirstName))
                throw new JsonException("Некорректные данные JSON.");
            return student;
        }

        public static void SaveToFile(Student student, string filePath)
        {
            string json = Serialize(student);
            File.WriteAllText(filePath, json);
        }

        public static Student LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return Deserialize(json);
        }
    }
}