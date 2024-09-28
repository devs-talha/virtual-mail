using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Project.Models
{
    public class UserDAO
    {
        public User GetUser(string email)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "select id, first_name, last_name, email, password, date_of_birth::timestamp without time zone as \"date_of_birth\" " +
                    "from public.user where email = @email";
                command.Parameters.AddWithValue("@email", email);
                NpgsqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    return new User(int.Parse(reader["id"].ToString()), 
                        DateTimeOffset.Parse(reader["date_of_birth"].ToString()).DateTime, reader["first_name"].ToString(), 
                        reader["last_name"].ToString(), reader["email"].ToString(), 
                        reader["password"].ToString());
                else
                    return null;
            }
        }

        public User GetUser(int id)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "select * from public.user where id = @id";
                command.Parameters.AddWithValue("@id", id);
                NpgsqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    return new User(int.Parse(reader["id"].ToString()),
                        DateTimeOffset.Parse(reader["date_of_birth"].ToString()).DateTime, reader["first_name"].ToString(),
                        reader["last_name"].ToString(), reader["email"].ToString(),
                        reader["password"].ToString());
                else
                    return null;
            }
        }

        public bool UpdateUser(User user)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "update public.user set first_name = @first_name, last_name = @last_name, " +
                    " date_of_birth = @date_of_birth " +
                    "where email = @email";
                command.Parameters.AddWithValue("@first_name", user.FirstName);
                command.Parameters.AddWithValue("@last_name", user.LastName);
                command.Parameters.AddWithValue("@date_of_birth", user.DateOfBirth);
                command.Parameters.AddWithValue("@email", user.Email);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool InsertUser(User user)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "insert into public.user (first_name, last_name, " +
                    "date_of_birth, password, email) values (@first_name, @last_name, " +
                    "@date_of_birth, @password, @email)";
                command.Parameters.AddWithValue("@first_name", user.FirstName);
                command.Parameters.AddWithValue("@last_name", user.LastName);
                command.Parameters.AddWithValue("@date_of_birth", user.DateOfBirth);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@email", user.Email);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteUser(User user)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "delete from public.user " +
                    "where email = @email";
                command.Parameters.AddWithValue("@email", user.Email);
                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
