using System;

namespace FriendsAppNoORM.Models
{
    public class ProfileReference
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ProfileReference(long id, string name){
            this.Id = id;
            this.Name = name;
        }
        public ProfileReference(long id)
        {
            this.Id = id;
        }

        public ProfileReference(){
            
        }

    }

}