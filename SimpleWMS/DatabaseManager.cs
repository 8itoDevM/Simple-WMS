﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SimpleWMS
{
    public class DatabaseManager
    {
        private string connectionString;

        public DatabaseManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public void CountItems()
        {
            int totalItems = 0;
            string query = "SELECT itemQty, itemName FROM Items";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totalItems += reader.GetInt32(0);
                            Console.WriteLine(reader.GetString(1) + ": " + reader.GetInt32(0));
                        }
                        Console.WriteLine($"There are {totalItems} items in stock");
                    }
                }
            }
        }

        public void ShowUsers()
        {
            string query = "SELECT Name FROM Users";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Showing all users");
                        while (reader.Read())
                        {
                            Console.WriteLine(reader.GetString(0));
                        }
                    }
                }
            }
        }

        public void RegisterItems(string itemName, int itemQty)
        {
            string query = "INSERT INTO Items (itemName, itemQty) VALUES (@itemName, @itemQty)";
            string verifySelect = "SELECT itemName from Items where itemName = @itemName";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(verifySelect, connection))
                {
                    command.Parameters.AddWithValue("@itemName", itemName);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Console.WriteLine("Err: Item already exists");
                        } else
                        {
                            reader.DisposeAsync();
                            using (MySqlCommand commandQuery = new MySqlCommand(query, connection))
                            {
                                commandQuery.Parameters.AddWithValue("@itemName", itemName);
                                commandQuery.Parameters.AddWithValue("@itemQty", itemQty);

                                commandQuery.ExecuteNonQuery();
                                Console.WriteLine("Item inserted successfully.");
                            }
                        }
                    }
                }
            }
        }

        public void AddItem(string itemName, int itemQty)
        {
            string query = "UPDATE Items SET itemQty = itemQty + @itemQty WHERE itemName = @itemName";
            string verifySelect = "SELECT itemName FROM Items WHERE itemName = @itemName";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(verifySelect, connection))
                {
                    command.Parameters.AddWithValue("@itemName", itemName);
                    command.Parameters.AddWithValue("@itemQty", itemQty);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Err: Item doesn't exists");
                        }
                        else if (reader.HasRows)
                        {
                            reader.DisposeAsync();
                            using (MySqlCommand commandUpdate = new MySqlCommand(query, connection))
                            {
                                commandUpdate.Parameters.AddWithValue("@itemName", itemName);
                                commandUpdate.Parameters.AddWithValue("@itemQty", itemQty);

                                commandUpdate.ExecuteNonQuery();
                                Console.WriteLine("Item inserted successfully!");
                            }
                        }
                    }
                }
            }
        }

        public void ShipItem(string itemName, int itemQty)
        {
            string query = "UPDATE Items SET itemQty = itemQty - @itemQty WHERE itemName = @itemName";
            string verifySelect = "SELECT itemName, itemQty from Items where itemName = @itemName";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(verifySelect, connection))
                {
                    command.Parameters.AddWithValue("@itemName", itemName);
                    command.Parameters.AddWithValue("@itemQty", itemQty);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Err: Item doesn't exists");
                        } else
                        {
                            reader.Read();

                            if (reader.GetInt32(1) >= itemQty)
                            {
                                reader.DisposeAsync();
                                using (MySqlCommand commandUpdate = new MySqlCommand(query, connection))
                                {
                                    commandUpdate.Parameters.AddWithValue("@itemName", itemName);
                                    commandUpdate.Parameters.AddWithValue("@itemQty", itemQty);

                                    commandUpdate.ExecuteNonQuery();
                                    Console.WriteLine("Item shipped successfully!");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Err: Not enough of {reader.GetString(0)} in stock - {reader.GetInt32(1)} left");
                            }
                        }
                    }
                }
            }
        }

        public void RemoveItems(string itemName)
        {
            string queryDelete = "DELETE FROM Items WHERE itemName = @itemName";
            string verifySelect = "SELECT itemId, itemName, itemQty FROM Items WHERE itemName = @itemName";
            string updateIds = "UPDATE Items SET itemId = itemId - 1 WHERE itemId > @deletedItemId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(verifySelect, connection))
                {
                    command.Parameters.AddWithValue("@itemName", itemName);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Err: Item doesn't exist");
                            return;
                        }

                        reader.Read();
                        int deletedItemId = reader.GetInt32(0); // Get the itemId of the item to be deleted
                        reader.Close();

                        // Delete the item
                        using (MySqlCommand commandDelete = new MySqlCommand(queryDelete, connection))
                        {
                            commandDelete.Parameters.AddWithValue("@itemName", itemName);
                            commandDelete.ExecuteNonQuery();
                            Console.WriteLine("Item Removed successfully!");
                        }

                        // Update the itemId of items that come after the deleted item
                        using (MySqlCommand commandUpdate = new MySqlCommand(updateIds, connection))
                        {
                            commandUpdate.Parameters.AddWithValue("@deletedItemId", deletedItemId);
                            commandUpdate.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
