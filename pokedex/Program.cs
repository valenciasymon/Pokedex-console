using Microsoft.Win32;
using System.Reflection;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace pokedex
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] menu =
                [
                    "1. View all Pokémon",
                    "2. Search Pokémon",
                    "3. Register Pokémon",
                    "4. Favorite a Pokémon",
                    "5. Delete a Pokémon",
                    "6. Exit"
                ];

            Pokedex(menu);

            Console.ReadKey();
        }


        static void Pokedex(string[] menu)
        {
            while (true)
            {
                DisplayMenu(menu);
                int userInput = NumCheck("Enter 1 - 5: ", 1, 6);

                switch (userInput)
                {
                    case 1:
                        ViewAll();
                        break;
                    case 2:
                        Search();
                        break;
                    case 3:
                        Register();
                        break;
                    case 4:
                        Favorite();
                        break;
                    case 5:
                        DeletePokemon();
                        break;
                    case 6:
                        Console.WriteLine("exiting ... ");
                        return;
                }
            }
        }

        static void DisplayMenu(string[] menu)
        {
            foreach (string item in menu)
            {
                Console.WriteLine(item);
            }
        }

        static string ValidString(string message)
        {
            string input;

            do
            {
                Console.WriteLine(message);
                input = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(input));

            return input.Trim();
        }

        static int NumCheck(string message,int min,int max)
        {
            int num = 0;

            while (true)
            {
                Console.WriteLine(message);
                string userInput = Console.ReadLine();

                if(int.TryParse(userInput, out num))
                {
                    if (num >= min && num <= max)
                    {
                        return num;
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("please enter a valid number");
            }
        }

        static void ViewAll()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string countqry = "SELECT COUNT(*) FROM pokemons";
                    MySqlCommand countcmd = new MySqlCommand(countqry, conn);

                    int count = Convert.ToInt32(countcmd.ExecuteScalar());

                    if (count == 0)
                    {
                        Console.WriteLine("No Pokemon Listed!");
                        return;
                    }

                    string query = "SELECT * FROM pokemons";
                    MySqlCommand cmd = new MySqlCommand(query,conn);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine($"Name: {reader["pokemon_name"]} | " +$"Element: {reader["element"]} | " +
                        $"Rarity: {reader["rarity"]} | "+ $"Favorite: {reader["isfavorite"]}");
                    }

                    Console.WriteLine();
                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
           
        }

        static void Search()
        {
            try
            {
                string name = ValidString("Enter Pokemon Name: ");
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    bool isListed = false;

                    string query = "SELECT * FROM pokemons WHERE pokemon_name = @name";

                    MySqlCommand cmd = new MySqlCommand(query,conn);

                    cmd.Parameters.AddWithValue("@name",name);

                    MySqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        isListed = true;
                        Console.WriteLine($"Name: {reader["pokemon_name"]} | " + $"Element: {reader["element"]} | " +
                        $"Rarity: {reader["rarity"]} | " + $"Favorite: {reader["isfavorite"]}");
                    }

                    if (!isListed)
                    {
                        Console.WriteLine("Pokemon not found");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
        }


        static void Register()
        {
            
            try
            {

                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string name = ValidString("Enter Pokemon Name: ");
                    string element = ValidString("Enter Pokemon Element: ");
                    string rarity = ValidString("Enter Pokemon Rarity: ");

                    string query = $"INSERT INTO pokemons(pokemon_name, element, rarity, isfavorite)" +
                        $"VALUES(@name, @element, @rarity, @fav);";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@element", element);
                    cmd.Parameters.AddWithValue("@rarity", rarity);
                    cmd.Parameters.AddWithValue("@fav", false);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " +e.Message);
            }
        }


        static void Favorite()
        {
            ViewAll();
            string name = ValidString("Enter Name to Favorite Pokemon : ");
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string query = "UPDATE pokemons SET isfavorite = true WHERE pokemon_name = @name";
                    MySqlCommand cmd = new MySqlCommand(query,conn);

                    cmd.Parameters.AddWithValue("@name",name);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        Console.WriteLine("Pokemon is now Favorited");
                    }
                    else
                    {
                        Console.WriteLine("No Pokemon Found");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " +e.Message);
            }
        }

        static void DeletePokemon()
        {
            ViewAll();
            string name = ValidString("Enter Pokemon Name");
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM pokemons WHERE pokemon_name = @name";

                    MySqlCommand cmd = new MySqlCommand(query,conn);

                    cmd.Parameters.AddWithValue("@name",name);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        Console.WriteLine("Pokemon is Deleted");
                    }
                    else
                    {
                        Console.WriteLine("Pokemon Not Found");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
        }

        static MySqlConnection GetConnection()
        {
            // connection info (where + who + which DB)
            string strconn = "server=localhost;user=root;password=YOURPASSWORD;database=pokedex;";
            return new MySqlConnection(strconn);
        }
    }
}
