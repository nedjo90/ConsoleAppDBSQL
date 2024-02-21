using MySql.Data.MySqlClient;

namespace ConsoleAppDBSQL;


class Program
{
    static void Main(string[] args)
    {
        DBConnect dbConnect = new DBConnect();
        List<string>[] selectAll = dbConnect.Select();
        for (int i = 0; i < selectAll[0].Count(); i++)
        {
            for (int j = 0; j < selectAll.Length; j++)
            {
                Console.Write(selectAll[j][i] + " ");
            }
            Console.WriteLine();
        }
    }
}