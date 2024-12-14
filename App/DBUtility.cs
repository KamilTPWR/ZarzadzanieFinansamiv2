using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;

namespace ZarzadzanieFinansami;

public abstract class DbUtility
{
    public static List<Transaction> GetFromDatabase(string dataBaseName = $"FinanseDataBase.db")
    {
        string command = "SELECT * FROM ListaTranzakcji";
        //ReSharper disable once UseCollectionExpression, ponieważ po co komplikować proste rzeczy
        List<string> columns = new List<string> {"ID", "Nazwa", "Kwota", "Data", "Uwagi" };
        
        List<Transaction> transactions = new();
        SQLitePCL.Batteries.Init();

        using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
        {
            try
            {
                connection.Open();
                var sqliteCommand = connection.CreateCommand();
                sqliteCommand.CommandText = command;

                using (var reader = sqliteCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = IfNotNull<int>("ID", columns, reader);
                        string name = IfNotNull<string>("Nazwa", columns, reader);
                        double amount = IfNotNull<double>("Kwota", columns, reader);
                        string date = IfNotNull<string>("Data", columns, reader);
                        string remarks = IfNotNull<string>("Uwagi", columns, reader);
                        transactions.Add(new Transaction(id,name, amount, date, remarks));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nie spodziewany bład GetFromDatabase", MessageBoxButton.OK, MessageBoxImage.Error);
                connection.Close();
            }
        }

        return transactions;
    }
    
    public static void SaveTransaction(string nazwa, string kwotaText, string data, string uwagi, 
        string dataBaseName = $"FinanseDataBase.db")
    {
        if (double.TryParse(kwotaText, out var kwota))
        {
            SQLitePCL.Batteries.Init();

            try
            {

                using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText =
                        "INSERT INTO ListaTranzakcji(Nazwa, Kwota, Data, Uwagi) VALUES ($nazwa, $kwota, $data, $uwagi)";
                    command.Parameters.AddWithValue("$nazwa", nazwa);
                    command.Parameters.AddWithValue("$kwota", kwota);
                    command.Parameters.AddWithValue("$data", data);
                    command.Parameters.AddWithValue("$uwagi", uwagi);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nie spodziewany bład SaveTransaction", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Invalid input for Kwota. Please enter a numeric value.");
        }
    }

    public static int GetNumberOfTransactions(string dataBaseName = $"FinanseDataBase.db")
    {
        List<Transaction> transactions = GetFromDatabase(dataBaseName);
        var i = transactions.Count;
        return i;
    }
    
    public static void DeleteFromDatabase(int index, string dataBaseName = $"FinanseDataBase.db",
        string tableName = $"ListaTranzakcji")
    {
        var command = $"DELETE FROM {tableName} WHERE ID = {index}";
        SQLitePCL.Batteries.Init();

        using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
        {
            try
            {
                connection.Open(); 
                var sqliteCommand = connection.CreateCommand();
                sqliteCommand.CommandText = command;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nie spodziewany bład", MessageBoxButton.OK, MessageBoxImage.Error);
                connection.Close();
            }
        }
    }
    
    private static dynamic IfNotNull<T>(string condition, List<string> columns, SqliteDataReader? reader)
    {
        if (reader == null) throw new NullReferenceException();
        if (typeof(T) == typeof(double))
        {
            var temp = columns.Contains(condition) && !reader.IsDBNull(columns.IndexOf(condition))
                ? reader.GetDouble(columns.IndexOf(condition))
                : 0;
            return temp;
        }

        if (typeof(T) == typeof(string))
        {
            var temp = (columns.Contains(condition) && !reader.IsDBNull(columns.IndexOf(condition))
                ? reader.GetString(columns.IndexOf(condition))
                : null) ?? string.Empty;
            return temp;
        }

        if(typeof(T) == typeof(int))
        {
            var temp = columns.Contains(condition) && !reader.IsDBNull(columns.IndexOf(condition))
                ? reader.GetInt32(columns.IndexOf(condition))
                : 0;
            return temp;
        }

        throw new NotSupportedException($"The type {typeof(T).Name} is not supported.");
    }
}