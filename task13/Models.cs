using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public class Subject
    {
        public string Name {get;set;}
        public int Grade {get; set;}
    }

    public class Student
    {
        public string FirstName {get;set;}
        public string LastName {get;set;}

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime BirthDate {get; set;}

        public List<Subject> Grades {get; set;}
    }


    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "dd.MM.yyyy";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), _format,null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}