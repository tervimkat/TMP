using System;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        public static SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5UQRBT8H;Initial Catalog=mydb;Integrated Security=True");

        static void Main(string[] args)
        {
            bool b = true;
            int ch = 0;
            while (b)
            {
                Console.WriteLine(
                "1) хранимая функция\n" +
                "2) хранимая процедура\n" +
                "3) демонстрация пользовательского типа\n" +
                "4) выход\n");
                try
                {
                    ch = Convert.ToInt32(Console.ReadLine());
                    if (!(ch > 0 && ch < 5))
                        Console.WriteLine("Неверный ввод");
                }
                catch
                {
                    Console.WriteLine("Неверный ввод");
                }
                switch (ch)
                {
                    case 1:
                        ExternalFunction();
                        break;
                    case 2:
                        ExternalProcedure();
                        break;
                    case 3:
                        ForType();
                        break;
                    case 4:
                        b = false;
                        break;
                }
            }
        }

        public static void ExternalFunction()
        {
            Console.WriteLine("Введите название фирмы велосипеда");
            string sqlExpression = $"SELECT * FROM GetCountByFirm('{Console.ReadLine()}')";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            da.Fill(dataSet);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            Console.WriteLine("количество: " + dataSet.Tables[0].Rows[0][dataSet.Tables[0].Columns[0]]);
            
        }

        public static void ExternalProcedure()
        {
            string sqlExpression = $"EXEC GetRents";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            da.Fill(dataSet);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            Console.WriteLine("Отчет:\n");
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                DataTable thisTable = dataSet.Tables[i];

                for (int j = 0; j < thisTable.Rows.Count; j++)
                {
                    DataRow row = thisTable.Rows[j];
                    Console.WriteLine("Велосипед: " + row[thisTable.Columns[0]] + " Бренд: " + row[thisTable.Columns[1]] + " Цена оренды: " + row[thisTable.Columns[2]] + " ФИО клиента: " + row[thisTable.Columns[3]]);

                }
            }

        }
        public static void ForType()
        {
            // берем последнего добавленного клиента и последний добавленный велосипед
            Random rnd = new Random(DateTime.Now.Second * DateTime.Now.Millisecond * DateTime.Now.Minute);
            string sqlExpression = "if (EXISTS (select top 1 * from Client) and EXISTS (select top 1 * from Bicycle))" +
                " begin " +
                    "declare @perem as MyTableType " +
                    "insert into @perem values ((select top 1 id from Bicycle ORDER BY id desc)," +
                                                   "(select top 1 id from Client ORDER BY id desc))," +
                                                   $"'{DateTime.Now}';" +
                    "insert into Orders values((select top 1 BicycleId from @perem)," +
                                                   "(select top 1 ClientiD from @perem)," +
                                                   "(select top 1 OrderDate from @perem)" +
                                                   $"{rnd.Next(1000, 10000)});" +
                 "end";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

    }
}
