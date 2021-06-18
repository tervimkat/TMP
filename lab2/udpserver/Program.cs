using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace UdpClientApp
{
    class Program
    {
        private const string remoteAddress = "127.0.0.1";
        // порты должны быть "зеркальны" для сервера и клиента
        private const int remotePort = 8001; // порт для отправки данных
        private const int localPort = 8002; // локальный порт для прослушивания входящих подключений

        static void Main(string[] args)
        {
            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                Console.WriteLine("Введите путь файла для отправки");
                string path = Console.ReadLine(); // переводим название и расширение файла в байты
                byte[] name = Encoding.UTF8.GetBytes(path.Split('\\')[path.Split('\\').Length - 1]);
                byte[] data = File.ReadAllBytes(path);

                sender.Send(name, name.Length, remoteAddress, remotePort);  // отправляем данные о названии
                sender.Send(data, data.Length, remoteAddress, remotePort);  // отправляем сами даныее файла

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();  // закрываем соединение
            }
        }
    }
}