using System;
using System.Data;
using FriendsAppNoORM.Models;
using MySqlConnector;

namespace FriendsAppNoORM.Data
{

    public class AccountDataSet : DataSet
    {
        public AccountDataSet(string connectionString) : base(connectionString)
        {
        }


        public bool IsTableEmpty()
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT EXISTS (SELECT 1 FROM account);";
                    cmd.CommandType = CommandType.Text;

                    long result = (long)cmd.ExecuteScalar();

                    return (result == 0);
                }
            }
        }
        public long Count()
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(accountid) FROM account;";
                    cmd.CommandType = CommandType.Text;

                    long result = (long)cmd.ExecuteScalar();

                    return result;
                }
            }
        }
        public Account Create(Account a)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO account (email, passwordhash) VALUES (@email, @passwordhash);";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@email", a.Email);
                    cmd.Parameters.AddWithValue("@passwordhash", a.PasswordHash);
                    
                    cmd.ExecuteNonQuery();
                }
            }

            Account created = this.RerieveByEmail(a.Email);

            return created;
        }

        public Account RerieveByEmail(string email)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT BIN_TO_UUID(accountid) AS accountid, profileid, email, passwordhash, changedon FROM account                                    
                                    WHERE email = @email";

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@email", email);

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        Account a = new Account();

                        a.AccountId = rdr.GetGuid("accountid");

                        if (!rdr.IsDBNull("profileid"))
                            a.ProfileId = rdr.GetInt32("profileid");

                        a.Email = rdr.GetString("email");
                        a.PasswordHash = rdr.GetString("passwordhash");                        
                        a.ChangedOn = rdr.GetDateTime("changedon");

                        return a;
                    }
                }
            }
            return null;
        }

        public bool Exists(string email)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT EXISTS (SELECT 1 FROM account where email = @email);";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@email", email);

                    long result = (long)cmd.ExecuteScalar();

                    return (result != 0);
                }
            }
        }
    }

}