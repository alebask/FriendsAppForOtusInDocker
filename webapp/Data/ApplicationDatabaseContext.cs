using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using FriendsAppNoORM.Models;
using MySqlConnector;

namespace FriendsAppNoORM.Data
{
    public class ApplicationDatabaseContext: IDisposable
    {
        private string _connectionString;
        
        public ApplicationDatabaseContext(string connectionString)
        {
            _connectionString = connectionString;      

            this.Account = new AccountDataSet(_connectionString);
            this.Profile = new ProfileDataSet(_connectionString);
            this.Friendship = new FriendshipDataSet(_connectionString);

            this.Page = new PageDataSet(_connectionString);
        }

        public AccountDataSet Account { get; private set; }
        public ProfileDataSet Profile { get; private set; }
        
        public PageDataSet Page {get; private set;}
        public FriendshipDataSet Friendship { get; private set; }
         
        public void Dispose()
        {
            
        }
    }
}
