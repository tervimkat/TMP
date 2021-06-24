using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace tcpService
{
    public partial class Service1 : ServiceBase
    {
        const int port = 8888;
        TcpListener server = null;
        Thread myThread;
        bool stop = false;
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
        }

        protected override void OnStart(string[] args)  // метод во время старта службы
        {
            myThread = new Thread(DoWordk);  // запускаем поток который будет выполнять основную работу 
            myThread.Start();
        }

        protected override void OnStop()
        {
            stop = true;
            TcpClient client = new TcpClient(); // создаем экземпляр класса для подключения, чтобы поток вышел из ожидания
            client.Connect("127.0.0.1", port);
            client.Close();
            server.Stop(); // останавлиеам сервер

        }

        public void DoWordk()
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1"); // создаем экземпляр класса для подключения
                server = new TcpListener(localAddr, port);
                server.Start();

                while (!stop)
                {
                    TcpClient client = server.AcceptTcpClient();  // ожидаем клиента
                    if (!stop)  // если служба не останавливается
                    {
                        // получаем название файла с расширением и преобразуем его в байты
                        string path = "C:\\Users\\TEST\\Desktop\\Новый текстовый документ.txt"; 
                        byte[] name = Encoding.UTF8.GetBytes(path.Split('\\')[path.Split('\\').Length - 1]);
                        byte[] data = File.ReadAllBytes(path);
                        Array.Resize(ref name, 256);

                        NetworkStream stream = client.GetStream(); // создаем поток записи в подлкючение
                        stream.Write(name, 0, 256);// отправляем клиенту 256 байт информации, где храниться названеи файла
                        stream.Write(data, 0, data.Length); // отправляем клиенту информацию самого файла
                        stream.Close();  // закрываем поток записи 
                    }
                    client.Close(); // закрываем tcp соединение
                }
            }
            catch
            {
            }
        }
    }
}
