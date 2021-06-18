using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

namespace npclient
{
    [Serializable]
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
    public class CustomBinder : SerializationBinder // метод для подмены сборки текущего приложения на сборку приложения передаваемого данные
    {
        // без этого метода нельзя сделать бинарную сериализацию, BinaryFormatter думает, что у клиента неправильное описание класса
        public override Type BindToType(string assemblyName, string typeName)
        {
            Assembly currentasm = Assembly.GetExecutingAssembly();

            return Type.GetType($"{currentasm.GetName().Name}.{typeName.Split('.')[1]}");
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
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Binder = new CustomBinder();
                    // получаем данные из канала и десериализуем
                    Person[] deserilizePeople = (Person[])formatter.Deserialize(pipeClient);
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
