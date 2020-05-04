using SimpleCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SimpleCRUD.Data
{

    public class CrudRepository : ICrudRepository
    {
        private readonly string _connectionString;
        public CrudRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<bool> Delete(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            string sqlQuery = "DELETE FROM [user] WHERE UId = @id";

            SqlCommand command = new SqlCommand(sqlQuery, conn);

            command.Parameters.Add("@id", System.Data.SqlDbType.Int);


            command.Parameters["@id"].Value = id;

            conn.Open();

            command.ExecuteNonQuery();

            conn.Close();

            return true;

        }


        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
           SqlConnection conn = new SqlConnection(_connectionString);

            string sqlQuery = "SELECT * FROM [user]";

            SqlCommand command = new SqlCommand(sqlQuery, conn);

            

            SqlDataAdapter sda = new SqlDataAdapter();

            sda.SelectCommand = command;

            conn.Open();
            SqlDataReader sdr = command.ExecuteReader();
            while(sdr.Read())
            {
                User obje = new User();
                obje.Name = sdr["Name"].ToString();
                obje.Address = sdr["Address"].ToString();
                users.Add(obje);
            }

            conn.Close();

            return users;
        }
        public async Task<User> Register(User user)
        {

            SqlConnection conn = new SqlConnection(_connectionString);

            string sqlQuery = "INSERT INTO [user] (Name,Address) VALUES (@name,@address)";

            SqlCommand command = new SqlCommand(sqlQuery, conn);

            command.Parameters.Add("@name", SqlDbType.VarChar);
            command.Parameters.Add("@address", SqlDbType.VarChar);


            command.Parameters["@name"].Value = user.Name;
            command.Parameters["@address"].Value = user.Address;

            conn.Open();

            command.ExecuteNonQuery();

            conn.Close();

            return user;

        }
    }
}
