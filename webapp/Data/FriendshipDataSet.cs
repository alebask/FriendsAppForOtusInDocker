using System;
using System.Collections.Generic;
using System.Data;
using FriendsAppNoORM.Models;
using MySqlConnector;

namespace FriendsAppNoORM.Data
{
    public class FriendshipDataSet : DataSet
    {
        public FriendshipDataSet(string connectionString) : base(connectionString)
        {
        }

        public int Create(Friendship r)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO friendship (requestedby, requestedto, status) 
                                    VALUES (@requestedby, @requestedto, @status);";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@requestedby", r.RequestedBy.Id);
                    cmd.Parameters.AddWithValue("@requestedto", r.RequestedTo.Id);

                    cmd.Parameters.AddWithValue("@status", ((long)r.Status));

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public Friendship Retrieve(long requestedBy, long RequestedTo)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT requestedby, requestedto, status, changedon FROM friendship
                                        WHERE requestedby = @requestedby AND requestedto = @requestedto";

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@requestedby", requestedBy);
                    cmd.Parameters.AddWithValue("@requestedto", RequestedTo);

                    MySqlDataReader rdr = cmd.ExecuteReader();


                    if (rdr.Read())
                    {
                        Friendship fr = new Friendship();
                        fr.RequestedBy = new ProfileReference(rdr.GetInt64("requestedby"));
                        fr.RequestedTo = new ProfileReference(rdr.GetInt64("requestedto"));
                        fr.Status = (FriendshipStatus)rdr.GetInt64("status");
                        fr.ChangedOn = rdr.GetDateTime("changedon");

                        return fr;

                    }
                }
            }

            return null;
        }

        public int UpdateStatus(Friendship fr)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE friendship SET status = @status
                                        WHERE requestedby = @requestedby AND requestedto = @requestedto";

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@requestedby", fr.RequestedBy.Id);
                    cmd.Parameters.AddWithValue("@requestedto", fr.RequestedTo.Id);
                    cmd.Parameters.AddWithValue("@status", ((long)fr.Status));

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Friendship> RetrieveAllForProfile(long profileId, FriendshipStatus status )
        {
            List<Friendship> output = new List<Friendship>();

            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT requestedby, p1.firstname AS 'requestedbyfirstname', p1.lastname AS 'requestedbylastname',
                                    requestedto, p2.firstname AS 'requestedtofirstname', p2.lastname AS 'requestedtolastname',
                                    f.changedon 
                                    FROM friendship AS f
                                    INNER JOIN profile AS p1 ON p1.profileid = f.requestedby
                                    INNER JOIN profile AS p2 ON p2.profileid = f.requestedto
                                    WHERE (requestedby = @profileid OR requestedto = @profileid) AND status = @status";

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@profileid", profileId);
                    cmd.Parameters.AddWithValue("@status", (long)status);

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Friendship fr = new Friendship();
                        
                        fr.ChangedOn = rdr.GetDateTime("changedon");

                        long byid = rdr.GetInt64("requestedby");
                        string byfirstname = rdr.GetString("requestedbyfirstname");
                        string bylastname = rdr.GetString("requestedbylastname");

                        long toid = rdr.GetInt64("requestedto");
                        string tofirstname = rdr.GetString("requestedtofirstname");
                        string tolastname = rdr.GetString("requestedtolastname");


                        fr.RequestedBy = new ProfileReference(byid, $"{byfirstname} {bylastname}");
                        fr.RequestedTo = new ProfileReference(toid, $"{tofirstname} {tolastname}");


                        output.Add(fr);
                    }

                }
            }
            return output;
        }

        public int Delete(long requestedBy, long requestedTo)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM friendship 
                                WHERE requestedby = @requestedby AND requestedto = @requestedto";

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@requestedby", requestedBy);
                    cmd.Parameters.AddWithValue("@requestedto", requestedTo);                    

                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}