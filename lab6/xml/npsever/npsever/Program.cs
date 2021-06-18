using System;
using System.IO;
using System.IO.Pipes;
using System.Xml.Serialization;

namespace npsever
{
    
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
                Person person1 = new Person("Tom", 29);
                Person person2 = new Person("Bob", 35);
                Person[] people = new Person[] { person1, person2 };
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
                {
                    pipeServer.WaitForConnection();
                    XmlSerializer formatter = new XmlSerializer(typeof(Person[])); // класс xml сериализации
                    formatter.Serialize(pipeServer, people); // сериализуем и отправляем в канал
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
