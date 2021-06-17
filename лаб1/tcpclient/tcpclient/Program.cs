using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace tcpclient
{
    class Program
    {
        private const int port = 8888; // порт, должны совпадать
        private const string server = "127.0.0.1"; // ip адресс сервера

        static void Main(string[] args)
        {
            try
            {
                TcpClient client = new TcpClient(); // создаем экземпляр класса для подключения
                client.Connect(server, port); // пытаемся подключиться к серверу
                NetworkStream stream = client.GetStream(); // создаем поток чтения из подключения
                byte[] name = new byte[256]; // резервируем 256 байт для названия файла
                string namestr = Encoding.UTF8.GetString(name, 0, stream.Read(name, 0, name.Length)); // получаем название файла и переводим в строку
                namestr = namestr.Remove(namestr.IndexOf('\0')); // удаляем из строки лишние символы
                File.Delete(namestr);

                Stream fs = new FileStream(namestr, FileMode.Append, FileAccess.Write); // поток записи данных в файл
                do
                {
                    fs.WriteByte((byte)stream.ReadByte()); // получаем и сразу же записываем информацию по байтам
                }
                while (stream.DataAvailable); // пока данные есть в потоке
                fs.Close(); // закрываем поток записи в файл
                stream.Close(); // закрываем поток чтения из подключения
                client.Close(); // закрываем tcp соединение


            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            Console.WriteLine("Запрос завершен...");
            Console.Read();
        }
    }
}
