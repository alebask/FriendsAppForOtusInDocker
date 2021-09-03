using System;

namespace FriendsAppNoORM.Models
{
    public class Account
    {        
        public Guid AccountId { get; set; }
        public long? ProfileId { get; set; }
        public string Email { get; set; }
        public string PasswordHash {get;set;}
        public DateTime ChangedOn { get; set;}
    }
}