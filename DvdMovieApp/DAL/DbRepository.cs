using DvdMovieApp.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DvdMovieApp.DAL
{
    public class DbRepository
    {
        private readonly string _connectionString;
        public DbRepository()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<DbRepository>().Build();
            _connectionString = config.GetConnectionString("develop");
        }
        // CRUD
        #region Read
        public async Task<Film> GetFilmById(int id)
        {
            // hämta filmer från databas
            // ORM

            // Koppla upp mot databasen
            string stmt = "select * from film where film_id=@id";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("id", id);
            await using var reader = await command.ExecuteReaderAsync();
            Film film = new Film();
            while (await reader.ReadAsync())
            {
                film = new Film()
                {
                    Film_id = reader.GetInt32(0),
                    Title = (string)reader["title"]
                };
            }


            // Kopplingsträng
            // i den har vi vårt lösenord och användarnamn
            //https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows
            return film;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            // hämta filmer från databas
            // ORM

            // Koppla upp mot databasen
            string stmt = "select * from category where category_id = @id";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("id", id);
            await using var reader = await command.ExecuteReaderAsync();
            Category category = new Category();
            while (await reader.ReadAsync())
            {
                //if (reader["test"] == DBNull.Value)
                //{

                //}
                category = new()
                {
                    Category_id = reader.GetInt32(0),
                    Name = reader["name"] == DBNull.Value ? null : (string)reader["name"],
                    Test = ConvertFromDBVal<string>(reader["test"])
                };
            }


            // Kopplingsträng
            // i den har vi vårt lösenord och användarnamn
            //https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows
            return category;
        }
        // DRY
        // don't repeat yourself

        public async Task<IEnumerable<Film>> GetFilms()
        {
            List<Film> films = new List<Film>();
            // Koppla upp mot databasen
            string stmt = "select * from film";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            await using var reader = await command.ExecuteReaderAsync();
            Film film = new Film();
            while (await reader.ReadAsync())
            {
                film = new Film()
                {
                    Film_id = reader.GetInt32(0),
                    Title = (string)reader["title"],
                    //Language = new()
                    //{
                    //    Language_id= reader.GetInt32(1),
                    //    Name = (string)reader["name"]   
                    //}
                };
                films.Add(film);
            }

            return films;
        }
        
        
        #endregion



        // Utgå alltid från att allt i databasen kan gå på tok!
        public async Task AddCategory(Category category)
        {
            try
            {
                // Ni får aldrig skicka in parametrar på detta sätt i en databas!
                string stmt = $"insert into category(name) values ({category.Name})";

                stmt = "insert into category(name) values(@name)";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);

                await using var command = dataSource.CreateCommand(stmt);
                command.Parameters.AddWithValue("name", category.Name);
                await command.ExecuteNonQueryAsync();
            }
            catch (PostgresException ex)
            {
                string message = "Det blev fel i databasen";
                string errorCode = ex.SqlState;
                //if (errorCode == PostgresErrorCodes.StringDataRightTruncationWarning)
                //{
                //    message = "Namnet är för långt. Max 25 tecken";
                //}
                switch (errorCode)
                {
                    case PostgresErrorCodes.StringDataRightTruncation:
                        message = "Namnet är för långt. Max 25 tecken";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        message = "Namnet på kategorin måste vara unikt.";
                        break;
                    default:
                        break;
                }
                throw new Exception(message, ex);
            }
        }

        public async Task<Category> AddCategory2(Category category)
        {
            try
            {
               
                // Glömde ju korrigera frågan så att den returnerade primärnyckeln!!
                string stmt = "insert into category(name, test) values(@name, @test) returning category_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);

                await using var command = dataSource.CreateCommand(stmt);
                // en ifsats som handlar om värden i databasen bör undvikas
                // men, det är ok att använda dem mot datatyper
                //if (category.Test == null)
                //{
                //    command.Parameters.AddWithValue("test", DBNull.Value);
                //}
                //else
                //{
                //    command.Parameters.AddWithValue("test", category.Test);

                //}
                command.Parameters.AddWithValue("test", (object)category.Test??DBNull.Value);
                command.Parameters.AddWithValue("name", category.Name);
                
                category.Category_id =(int) await command.ExecuteScalarAsync();
                return category;
            }
            catch (PostgresException ex)
            {
                string message = "Det blev fel i databasen";
                string errorCode = ex.SqlState;
                //if (errorCode == PostgresErrorCodes.StringDataRightTruncationWarning)
                //{
                //    message = "Namnet är för långt. Max 25 tecken";
                //}
                switch (errorCode)
                {
                    case PostgresErrorCodes.StringDataRightTruncation:
                        message = "Namnet är för långt. Max 25 tecken";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        message = "Namnet på kategorin måste vara unikt.";
                        break;
                    default:
                        break;
                }
                throw new Exception(message, ex);
            }
            catch(Exception)
            {
                throw new Exception("ett annat fel");
            }
            
        }

        private static T? ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default;
            }
            return (T)obj;
        }

        private static object ConvertToDbVal<T>(object obj)
        {
            if (obj == null || obj == string.Empty)
            {
                return DBNull.Value;
            }
            return (T)obj;
        }

    }
}
