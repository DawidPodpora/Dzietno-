using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;

namespace Dzietność
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Wywołaj metodę asynchroniczną i poczekaj na jej zakończenie
            Task.Run(async () => await InitAsync()).GetAwaiter().GetResult();

            Application.Run(new Logowanie());
        }

        static async Task InitAsync()
        {
            // Twoje operacje asynchroniczne, np. połączenie z MongoDB
            // var client = new MongoClient("your_connection_string");
            // var database = client.GetDatabase("your_database_name");
            // await database.SomeAsyncOperation();

            // Dodaj tu inne operacje asynchroniczne, jeśli są potrzebne
        }
    }
}
