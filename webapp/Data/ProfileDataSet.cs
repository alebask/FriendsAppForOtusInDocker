using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FriendsAppNoORM.Models;
using MySqlConnector;

namespace FriendsAppNoORM.Data
{

    public class ProfileDataSet : DataSet
    {
        public ProfileDataSet(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<Profile>> RetrieveNotRelatedAsync(long profileId, string lastName, string firstName, int pageIndex, int pageSize)
        {
            List<Profile> output = new List<Profile>();

            using (var conn = await this.CreateOpenConnectionAsync())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT profileid, changedon, firstname, lastname, age, gender, city, interests FROM profile                                      
                                        WHERE profileid != @profileid AND                                   
                                        profileid NOT IN ( SELECT requestedto From friendship WHERE requestedby = @profileid) AND
                                        profileid NOT IN ( SELECT requestedby From friendship WHERE requestedto = @profileid)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@profileid", profileId);

                    //order is important due to changes in CommandText
                    AddNameFilter(cmd, lastName, firstName);
                    AddPagingFilter(cmd, pageIndex, pageSize);

                    MySqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (rdr.Read())
                    {
                        Profile p = new Profile();
                        p.ProfileId = rdr.GetInt64("profileid");                        
                        p.ChangedOn = rdr.GetDateTime("changedon");
                        p.FirstName = rdr.GetString("firstname");
                        p.LastName = rdr.GetString("lastname");
                        p.Age = rdr.GetInt32("age");
                        p.Gender = rdr.GetString("gender");
                        p.City = rdr.GetString("city");
                        p.Interests = rdr.GetString("interests");

                        output.Add(p);

                    }

                }
            }
            return output;
        }

        public List<Profile> RetrieveNotRelated(long profileId, string lastName, string firstName, int pageIndex, int pageSize)
        {
            List<Profile> output = new List<Profile>();

            using (var conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT profileid, changedon, firstname, lastname, age, gender, city, interests FROM profile                                      
                                        WHERE profileid != @profileid AND                                   
                                        profileid NOT IN ( SELECT requestedto From friendship WHERE requestedby = @profileid) AND
                                        profileid NOT IN ( SELECT requestedby From friendship WHERE requestedto = @profileid)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@profileid", profileId);

                    //order is important due to changes in CommandText
                    AddNameFilter(cmd, lastName, firstName);
                    AddPagingFilter(cmd, pageIndex, pageSize);

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Profile p = new Profile();
                        p.ProfileId = rdr.GetInt64("profileid");
                        p.ChangedOn = rdr.GetDateTime("changedon");
                        p.FirstName = rdr.GetString("firstname");
                        p.LastName = rdr.GetString("lastname");
                        p.Age = rdr.GetInt32("age");
                        p.Gender = rdr.GetString("gender");
                        p.City = rdr.GetString("city");
                        p.Interests = rdr.GetString("interests");

                        output.Add(p);

                    }

                }
            }
            return output;
        }

        private void AddPagingFilter(MySqlCommand cmd, int pageIndex, int pageSize)
        {
            cmd.CommandText += " ORDER BY profileid LIMIT @offset, @limit";
            cmd.Parameters.AddWithValue("@offset", pageIndex*pageSize);
            cmd.Parameters.AddWithValue("@limit", pageSize);
        }
    
        public async Task<long>  RetieveNotRelatedCountAsync(long profileId, string lastName, string firstName)
        {
            using (var conn = await this.CreateOpenConnectionAsync())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT COUNT(profileid) FROM profile                                      
                                        WHERE profileid != @profileid AND                                   
                                        profileid NOT IN ( SELECT requestedto From friendship WHERE requestedby = @profileid) AND
                                        profileid NOT IN ( SELECT requestedby From friendship WHERE requestedto = @profileid)";

                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@profileid", profileId);

                    AddNameFilter(cmd, lastName, firstName);

                    var result = await cmd.ExecuteScalarAsync();

                    return (long)result;
                }
            }
        }
        public long RetieveNotRelatedCount(long profileId, string lastName, string firstName)
        {
            using (var conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT COUNT(profileid) FROM profile                                      
                                        WHERE profileid != @profileid AND                                   
                                        profileid NOT IN ( SELECT requestedto From friendship WHERE requestedby = @profileid) AND
                                        profileid NOT IN ( SELECT requestedby From friendship WHERE requestedto = @profileid)";

                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@profileid", profileId);

                    AddNameFilter(cmd, lastName, firstName);

                    var result = cmd.ExecuteScalar();

                    return (long)result;
                }
            }
        }

        private void AddNameFilter(MySqlCommand cmd, string lastName, string firstName)
        {
            bool lastNameFilterApplied = !String.IsNullOrEmpty(lastName);
            bool firstNameFilterApplied = !String.IsNullOrEmpty(firstName);

            if(!lastNameFilterApplied && !firstNameFilterApplied){
                return;
            }

            if(lastNameFilterApplied){
                cmd.CommandText += " AND lastname LIKE @lastname";
                cmd.Parameters.AddWithValue("@lastname", lastName + "%");
            }
            if (firstNameFilterApplied)
            {
                cmd.CommandText += " AND firstname LIKE @firstname";
                cmd.Parameters.AddWithValue("@firstname", firstName + "%");
            }
        }

        public Profile Retrieve(long profileId)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT profileid, changedon, firstname, lastname, age, gender, city, interests FROM profile 
                                    WHERE profileid = @profileid";

                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@profileid", profileId);

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        Profile p = new Profile();
                        p.ProfileId = rdr.GetInt64("profileid");                        
                        p.ChangedOn = rdr.GetDateTime("changedon");
                        p.FirstName = rdr.GetString("firstname");
                        p.LastName = rdr.GetString("lastname");
                        p.Age = rdr.GetInt32("age");
                        p.Gender = rdr.GetString("gender");
                        p.City = rdr.GetString("city");
                        p.Interests = rdr.GetString("interests");


                        return p;
                    }

                }
            }
            return null;
        }

        public void Update(Profile p)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE profile SET lastname = @lastname, firstname = @firstname, age = @age, city= @city, gender = @gender, interests = @interests
                                   WHERE profileid = @profileid";

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@profileid", p.ProfileId);

                    cmd.Parameters.AddWithValue("@firstname", p.FirstName);
                    cmd.Parameters.AddWithValue("@lastname", p.LastName);
                    cmd.Parameters.AddWithValue("@age", p.Age);
                    cmd.Parameters.AddWithValue("@gender", p.Gender);
                    cmd.Parameters.AddWithValue("@city", p.City);
                    cmd.Parameters.AddWithValue("@interests", p.Interests);                    

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Profile Create(Guid accountId, Profile p)
        {
            using (MySqlConnection c = this.CreateOpenConnection())
            {
                using (MySqlTransaction t = c.BeginTransaction())
                {
                    DateTime now = DateTime.Now;

                    using (var cmd1 = c.CreateCommand())
                    {
                        cmd1.Transaction = t;
                        cmd1.CommandText = @"INSERT INTO profile (firstname, lastname, age, gender, city, interests) 
                                             VALUES (@firstname, @lastname, @age, @gender, @city, @interests);";

                        cmd1.CommandType = CommandType.Text;

                        cmd1.Parameters.AddWithValue("@firstname", p.FirstName);
                        cmd1.Parameters.AddWithValue("@lastname", p.LastName);
                        cmd1.Parameters.AddWithValue("@age", p.Age);
                        cmd1.Parameters.AddWithValue("@gender", p.Gender);
                        cmd1.Parameters.AddWithValue("@city", p.City);
                        cmd1.Parameters.AddWithValue("@interests", p.Interests);

                        cmd1.ExecuteNonQuery();

                        p.ProfileId = cmd1.LastInsertedId;
                    }

                    using (var cmd2 = c.CreateCommand())
                    {

                        cmd2.Transaction = t;
                        cmd2.CommandText = @"UPDATE account SET profileid = @profileid 
                                             WHERE accountid = UUID_TO_BIN(@accountid)";
                        cmd2.CommandType = CommandType.Text;

                        cmd2.Parameters.AddWithValue("@profileid", p.ProfileId);
                        cmd2.Parameters.AddWithValue("@accountid", accountId);

                        cmd2.ExecuteNonQuery();
                    }

                    try
                    {
                        t.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        t.Rollback();
                        throw ex;
                    }
                }
            }

            return p;
        }

    }

}