using System;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace npsever
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
                Person person1 = new Person("Tom", 29);
                Person person2 = new Person("Bob", 35);
                Person[] people = new Person[] { person1, person2 };
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
                {
                    pipeServer.WaitForConnection();
                    using (StreamWriter sw = new StreamWriter(pipeServer)) // поток для записи в канал
                    {
                        foreach (var elem in people)
                        {
                            sw.WriteLine(JsonSerializer.Serialize(elem)); // сериализуем и отправляем
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
