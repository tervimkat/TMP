using System;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

namespace npclient
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.In))
                {
                    pipeClient.Connect();
                    using (StreamReader sr = new StreamReader(pipeClient)) // поток для принятия данных
                    {
                        string temp;
                        while ((temp = sr.ReadLine()) != null)
                        {
                            Person p = JsonSerializer.Deserialize<Person>(temp);
                            Console.WriteLine($"Имя: {p.Name} --- Возраст: {p.Age}");
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
