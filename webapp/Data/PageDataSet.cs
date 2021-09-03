using System;
using System.Collections.Generic;
using System.Data;
using FriendsAppNoORM.Models;
using MySqlConnector;

namespace FriendsAppNoORM.Data
{
    public class PageDataSet : DataSet
    {
        public PageDataSet(string connectionString) : base(connectionString)
        {
        }

        public List<Page> RetrieveAllForProfile(long profileId)
        {
            List<Page> output = new List<Page>();

            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT pageid, profileid, title, url, content, changedon FROM page 
                                       WHERE profileid = @profileid";

                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@profileid", profileId);

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while(rdr.Read())
                    {
                        Page p = new Page();
                        p.PageId = rdr.GetInt64("pageid");
                        p.Profile = new ProfileReference(rdr.GetInt64("profileid"));
                        p.ChangedOn = rdr.GetDateTime("changedon");
                        p.Content = rdr.GetString("content");
                        p.Title = rdr.GetString("title");
                        p.Url = rdr.GetString("url");
                        
                        output.Add(p);
                    }
                }
            }

            return output;
        }

        public object Retrieve(long profileId, string url)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT pg.pageid, pg.profileid, p.lastname, p.firstname, pg.title, pg.url, pg.content, pg.changedon FROM page AS pg
                                        INNER JOIN profile AS p ON p.profileid = pg.profileid
                                       WHERE pg.profileid = @profileid AND pg.url = @url";

                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@profileid", profileId);
                    cmd.Parameters.AddWithValue("@url", url);

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Page p = new Page();
                        p.PageId = rdr.GetInt64("pageid");

                        long id = rdr.GetInt64("profileid");
                        string firstName = rdr.GetString("firstname");
                        string lastName = rdr.GetString("lastname");

                        p.Profile = new ProfileReference(id, $"{firstName} {lastName}");
                        
                        
                        p.ChangedOn = rdr.GetDateTime("changedon");
                        p.Content = rdr.GetString("content");
                        p.Title = rdr.GetString("title");
                        p.Url = rdr.GetString("url");

                        return p;
                    }
                }
            }

            return null;
        }

        public long Create(Page page)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO page (profileid, title, url, content)
                                        VALUES (@profileid, @title, @url, @content)";

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@profileid", page.Profile.Id);
                    cmd.Parameters.AddWithValue("@title", page.Title);
                    cmd.Parameters.AddWithValue("@url", page.Url);
                    cmd.Parameters.AddWithValue("@content", page.Content);
                    
                    cmd.ExecuteNonQuery();

                    return cmd.LastInsertedId;

                }
            }
        }

        internal Page Retrieve(long pageId)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT pageid, profileid, title, url, content, changedon FROM page 
                                       WHERE pageid = @pageid";

                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@pageid", pageId);

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Page p = new Page();
                        p.PageId = rdr.GetInt64("pageid");
                        p.Profile = new ProfileReference( rdr.GetInt64("profileid"));
                        p.ChangedOn = rdr.GetDateTime("changedon");
                        p.Content = rdr.GetString("content");
                        p.Title = rdr.GetString("title");
                        p.Url = rdr.GetString("url");

                        return p;
                    }
                }
            }

            return null;
        }

        public long Delete(long pageId)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM page WHERE pageid = @pageid";                    
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@pageid", pageId);

                    long deleted = cmd.ExecuteNonQuery();

                    return deleted;
                }
            }
        }

        public long Update(Page page)
        {
            using (MySqlConnection conn = this.CreateOpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE page SET title = @title, url = @url, content = @content
                                        WHERE pageid = @pageid";

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@pageid", page.PageId);
                    cmd.Parameters.AddWithValue("@title", page.Title);
                    cmd.Parameters.AddWithValue("@url", page.Url);
                    cmd.Parameters.AddWithValue("@content", page.Content);

                    long updated = cmd.ExecuteNonQuery();

                    return updated;

                }
            }
        }
    }
}