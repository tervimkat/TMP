using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;

namespace npsever
{
    [Serializable] // атрибут для возможности сериализовать класс
    class Person
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
                Person[] people = new Person[] { person1, person2 }; // создаем массив экземпляров классов
                // создаем именованный канал для передачи сериализованных данных
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
                {
                    pipeServer.WaitForConnection(); // ожидаем подключение
                    BinaryFormatter formatter = new BinaryFormatter(); // создаем экз. класса для сериализации данных
                    formatter.Serialize(pipeServer, people); // передаем в именованном канале сериализованные данные
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
