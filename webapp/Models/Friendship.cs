using System;
using System.ComponentModel.DataAnnotations;

namespace FriendsAppNoORM.Models
{
    public class Friendship{

        public ProfileReference RequestedBy { get; set; }
        public ProfileReference RequestedTo { get; set; }
        
        
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ChangedOn { get; set;}

        public FriendshipStatus Status {get; set; }

    }

    public enum FriendshipStatus{
        
        Pending,
        Established      
   }

}