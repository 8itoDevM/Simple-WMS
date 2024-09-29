using System;
using System.Collections.Generic;
//using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SimpleWMS
{
    internal class Program
    {
        static int Menu()
        {
            int i;
            Console.WriteLine("Count items - 1");
            Console.WriteLine("Show users - 2");
            Console.WriteLine("Register item - 3");
            Console.WriteLine("Add items - 4");
            Console.WriteLine("Ship Item - 5");
            Console.WriteLine("Exit - 0");

            try
            {
                i = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                return -1;
            }

            Console.Clear();
            return i;
        }

        static void CountItems(DatabaseManager dbManager)
        {
            dbManager.CountItems();
            //Console.WriteLine($"There are {qty} items in stock");
        }

        static void ShowUsers(DatabaseManager dbManager)
        {
            dbManager.ShowUsers();
        }

        static void RegisterItem(DatabaseManager dbManager)
        {
            string itmName = "default";
            int itmInit = 0;

            Console.WriteLine("Registering an item");
            Console.WriteLine(" ");

            Console.Write("Item name: ");
            itmName = Console.ReadLine();

            Console.Write("Item initial quantity: ");
            itmInit = Convert.ToInt32(Console.ReadLine());

            dbManager.RegisterItems(itmName, itmInit);
        }

        static void AddItem(DatabaseManager dbManager)
        {
            string itmName = "default";
            int itmQty = 0;

            Console.WriteLine("Adding an Item");
            Console.WriteLine(" ");

            Console.Write("Item name: ");
            itmName = Console.ReadLine();

            Console.Write("Quantity to be added: ");
            itmQty = Convert.ToInt32(Console.ReadLine());

            dbManager.AddItem(itmName, itmQty);
        }

        static void ShipItems(DatabaseManager dbManager)
        {
            string itmName = "default";
            int itmQty = 0;

            Console.WriteLine("Shipping an Item");
            Console.WriteLine(" ");

            Console.Write("Item name: ");
            itmName = Console.ReadLine();

            Console.Write("Quantity to be shipped: ");
            itmQty = Convert.ToInt32(Console.ReadLine());

            dbManager.ShipItem(itmName, itmQty);
        }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("MySqlDatabase");

            DatabaseManager dbManager = new DatabaseManager(connectionString);

            bool running = true;

            while (running)
            {
                switch (Menu())
                {
                    case 0:
                        Console.Clear();
                        running = false;

                        break;
                    case 1:
                        Console.Clear();

                        CountItems(dbManager);

                        Console.ReadLine();
                        break;
                    case 2:
                        Console.Clear();

                        ShowUsers(dbManager);

                        Console.ReadLine();
                        break;
                    case 3:
                        Console.Clear();

                        RegisterItem(dbManager);

                        Console.ReadLine();
                        break;
                    case 4:
                        Console.Clear();

                        AddItem(dbManager);

                        Console.ReadLine();
                        break;
                    case 5:
                        Console.Clear();

                        ShipItems(dbManager);

                        Console.ReadLine();
                        break;
                    default:
                        Console.Clear();

                        Console.WriteLine("Invalid choice. Please try again.");

                        Console.ReadLine();
                        break;
                }

                Console.Clear();
            }

            Console.Write("Press any Key to exit ");
            Console.ReadLine();
        }
    }
}
