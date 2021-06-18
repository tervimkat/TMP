using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace udpclient
{
    class Program
    {
        private const string remoteAddress = "127.0.0.1";
        // порты должны быть "зеркальны" для сервера и клиента
        private const int remotePort = 8002; // порт для отправки данных
        private const int localPort = 8001; // локальный порт для прослушивания входящих подключений

        static void Main(string[] args)
        {
            UdpClient receiver = new UdpClient(localPort); // UdpClient для получения данных
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                byte[] name = receiver.Receive(ref remoteIp); // получаем название файла, а затем  переводим в строку
                string namestr = Encoding.UTF8.GetString(name, 0, name.Length);
                File.Delete(namestr);  // удаляем если есть такой файл уже

                Stream fs = new FileStream(namestr, FileMode.Append, FileAccess.Write); // поток записи в файл
                byte[] data = receiver.Receive(ref remoteIp);  // получаем всю информацию
                for (int i = 0; i < data.Length; i++)  // побайтово записываем ее в файл
                    fs.WriteByte(data[i]);
                fs.Close();  // закрываем поток записи в файл
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close(); // закрываем соединение
            }
        }
    }
}
