using System;
using System.IO;
using System.IO.Pipes;

namespace npclient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // создаем именованый канал и указываем, что будем получать данные
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.In))
                {

                    pipeClient.Connect(); // подключаемся 

                    using (StreamReader sr = new StreamReader(pipeClient)) // запускаем поток чтения из канала
                    {
                        string namestr = sr.ReadLine(); // в первой строке находится название файла
                        File.Delete(namestr); // удаляем, если такой файл уже есть 
                        Stream fs = new FileStream(namestr, FileMode.Append, FileAccess.Write); // поток записи в файл

                        string temp;
                        while ((temp = sr.ReadLine()) != null) // построчно записываем информацию в файл
                        {
                            fs.WriteByte(Convert.ToByte(temp));
                        }
                        fs.Close(); // закрываем поток записи в файл
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
