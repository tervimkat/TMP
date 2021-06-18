using System;
using System.IO;
using System.IO.Pipes;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace npsever
{
    [Serializable]
    public class Person : ISerializable
    {
        private int Id;
        public string Name { get; set; }
        public int Age { get; set; }
        [OptionalField]
        public string t;
        public Person(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
            t = "23";
        }
        public Person() { }
        protected Person(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            t = info.GetString("t");
            Age = info.GetInt32("Age");
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Age", Age);
            info.AddValue("t", t);
        }
        [OnDeserializing]
        private void SetValuesOnDeserializing(StreamingContext context)
        {
            Id = -1;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Person person1 = new Person(1, "Tom", 29);
                Person person2 = new Person(2, "Bob", 35);
                Person[] people = new Person[] { person1, person2 };
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
                {
                    pipeServer.WaitForConnection();
                    XmlSerializer formatter = new XmlSerializer(typeof(Person[]));
                    formatter.Serialize(pipeServer, people);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
