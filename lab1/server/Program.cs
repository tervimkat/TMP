using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcpserver
{
    class Program
    {
        const int port = 8888; // порт, должны совпадать
        static void Main(string[] args)
        {
            TcpListener server = null; // создаем экземпляр класса для прослушивания подключений
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1"); // в качестве ip указываем localhost
                server = new TcpListener(localAddr, port);
                server.Start(); // запускаем сервер
                
                while (true)
                {
                    Console.WriteLine("Ожидание подключения");
                    TcpClient client = server.AcceptTcpClient(); // ожидаем клиента

                    Console.WriteLine("Подключен клиент. Введите путь файла для отправки");
                    string path = Console.ReadLine(); // получаем название файла с расширением и преобразуем его в байты
                    byte[] name = Encoding.UTF8.GetBytes(path.Split('\\')[path.Split('\\').Length - 1]);
                    byte[] data = File.ReadAllBytes(path);
                    Array.Resize(ref name, 256); 


                    NetworkStream stream = client.GetStream(); // создаем поток записи в подлкючение
                    stream.Write(name, 0, 256); // отправляем клиенту 256 байт информации, где храниться названеи файла
                    stream.Write(data, 0, data.Length); // отправляем клиенту информацию самого файла
                    stream.Close(); // закрываем поток записи 
                    client.Close(); // закрываем tcp соединение
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server != null)
                    server.Stop(); // останавливаем сервер
            }
        }
    }
}
