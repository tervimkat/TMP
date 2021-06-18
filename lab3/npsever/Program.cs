using System;
using System.IO;
using System.IO.Pipes;

namespace npsever
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // создаем именованый канал и указываем, что будем отдавать данные
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
                {
                    Console.WriteLine("Ожидание подключения");
                    pipeServer.WaitForConnection(); // ожидаем клиента

                    Console.WriteLine("Подключен клиент. Введите путь файла для отправки");

                    string path = Console.ReadLine(); // считываем и передаем название файла
                    byte[] data = File.ReadAllBytes(path);

                    using (StreamWriter sw = new StreamWriter(pipeServer))  // создаем поток записи в канал
                    {
                        sw.WriteLine(path.Split('\\')[path.Split('\\').Length - 1]); // первой строкой отправим название и расширение
                        for (int i = 0; i < data.Length; i++) // построчно отправляем данные о файле
                            sw.WriteLine(data[i]);
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
