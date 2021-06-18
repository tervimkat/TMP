using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // создаем структуру как в длл
        [StructLayout(LayoutKind.Sequential)]
        public struct MyStruct
        {
            public long time;
            public double value;
        }
        // импортируем метод получения данных в структуру
        [DllImport(@"C:\Users\TEST\Desktop\ТМП\5лаб187-1ВорончихинИван\5лаб187-1ВорончихинИван\Dll\x64\Debug\Dll.dll")]
        public static extern MyStruct getStruct();

        public Chart myChart; // создаем график
        public delegate void AddChartItem(); // график занят методами отрисовки (главным потоком),
                                             // поэтому необходим делегат для отображения данных
        public AddChartItem myDelegate;
        public Thread MakeGraphThread; // поток добавления данных
        public Form1()
        {
            InitializeComponent();
            // создаем график
            myChart = new Chart();
            myChart.Parent = this;
            myChart.Dock = DockStyle.Fill;
            myChart.ChartAreas.Add(new ChartArea("Math functions"));
            Series mySeriesOfPoint = new Series("Sinus");
            mySeriesOfPoint.ChartType = SeriesChartType.Line;
            mySeriesOfPoint.ChartArea = "Math functions";
            myChart.Series.Add(mySeriesOfPoint);
            // к делегату привязываем метод
            myDelegate = new AddChartItem(MakeGraph);

        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            // запускаем поток отображения данных
            MakeGraphThread = new Thread(new ThreadStart(Run));
            MakeGraphThread.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // останавливаем поток отображения данных
            MakeGraphThread.Abort();
        }
        public void Run()
        {
            // метод для потока
            while (true)
            {
                // используя делегат, добавляем данные на график
                Invoke(myDelegate);
                Thread.Sleep(500);
            }
        }
        public void MakeGraph()
        {
            // получаем новые данные из длл
            MyStruct a = getStruct();
            // преобразуем int64 в datetime (на плюсах время хранится в секундах в типе long long)
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(a.time).ToLocalTime();
            if (myChart.Series[0].Points.Count > 20)
            {
                myChart.Series[0].Points.RemoveAt(0);
                myChart.Refresh();
            }
            myChart.Series[0].Points.AddXY(date.Minute * 60 + date.Hour * 3600 + date.Second, a.value);
            
        }

        
    }
}
