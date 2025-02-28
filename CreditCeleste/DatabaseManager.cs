using System;
using System.Data;
using System.Data.SqlClient;

namespace CreditCeleste
{
    public class DatabaseManager : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        // Le constructeur prend la cha�ne de connexion en param�tre
        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null");
        }

        // La m�thode ExecuteReader retourne maintenant un DataTable
        public DataTable ExecuteReader(string query, Action<SqlCommand> parameterAction = null)
        {
            var dataTable = new DataTable();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        parameterAction?.Invoke(command);
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
                return dataTable;
            }
            catch (SqlException ex)
            {
                // Journalisation des erreurs de base de donn�es
                LogDatabaseError(ex, query);
                throw;
            }
        }

        // La m�thode ExecuteQuery permet d'ex�cuter des requ�tes sans retour de donn�es
        public void ExecuteQuery(string query, Action<SqlCommand> parameterAction = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        parameterAction?.Invoke(command);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                // Journalisation des erreurs de base de donn�es
                LogDatabaseError(ex, query);
                throw; // Renvoi de l'exception pour permettre � l'appelant de la g�rer
            }
        }

        // M�thode de journalisation des erreurs de base de donn�es
        private void LogDatabaseError(Exception ex, string query)
        {
            // Impl�mentez ici le m�canisme de journalisation de votre choix
            Console.WriteLine($"Database Error: {ex.Message}");
            Console.WriteLine($"Query: {query}");
        }

        // Impl�mentation de la m�thode Dispose pour la gestion des ressources
        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}