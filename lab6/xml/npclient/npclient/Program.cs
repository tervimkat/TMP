using System;
using System.IO;
using System.IO.Pipes;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Reflection;

namespace npclient
{
    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Person() { }
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
                    XmlSerializer formatter = new XmlSerializer(typeof(Person[]));  // класс xml десериализации
                    Person[] deserilizePeople = (Person[])formatter.Deserialize(pipeClient);  // десериализуем данные из канала
                    foreach (Person p in deserilizePeople)
                    {
                        Console.WriteLine($"Имя: {p.Name} --- Возраст: {p.Age}");
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
