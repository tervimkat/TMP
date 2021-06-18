using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Reflection;

namespace npclient
{
    [Serializable]
    class Person : ISerializable
    {
        [NonSerialized()]
        public int Id;
        public string Name { get; set; }
        public int Age { get; set; }
        public string Country;
        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
        public Person() { }
        protected Person(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Age = info.GetInt32("Age");
            try
            {
                Country = info.GetString("Country");
            }
            catch { }
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Age", Age);
            try
            {
                info.AddValue("Country ", Country);
            }
            catch { }
        }
        [OnDeserializing]
        private void SetValuesOnDeserializing(StreamingContext context)
        {
            Country = "";
        }
    }
    public class CustomBinder : SerializationBinder
    {
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
                    Person[] deserilizePeople = (Person[])formatter.Deserialize(pipeClient);
                    foreach (Person p in deserilizePeople)
                    {
                        Console.WriteLine($"Id: {p.Id} --- Имя: {p.Name} --- Возраст: {p.Age} --- Страна: {p.Country}");
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
