using DvdMovieApp.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Film> GetFilm()
        {
        // hämta filmer från databas
        // ORM

        // Koppla upp mot databasen
        string stmt ="select * from film where film_id=1";
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);

        await using var command = dataSource.CreateCommand(stmt);
        await using var reader = await command.ExecuteReaderAsync();
         Film film= new Film();
        while(await reader.ReadAsync())
            {
                film = new Film()
                {
                    Film_id=reader.GetInt32(0),
                    Title = (string)reader["title"]
                };
            }


        // Kopplingsträng
        // i den har vi vårt lösenord och användarnamn
        //https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows
        return film;
        }
        
    }
}
