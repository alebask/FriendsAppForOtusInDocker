using System;

namespace FriendsAppDataGenerator
{

    //ID;Surname;Sex;PeoplesCount;WhenPeoplesCount;Source
    public class LastNameRecord
    {
        public string ID { get; set; }

        public string Surname { get; set; }
        public string Sex { get; set; }

        public double PeoplesCount { get; set; }

        public string WhenPeoplesCount { get; set; }

        public string Source { get; set; }

    }
}