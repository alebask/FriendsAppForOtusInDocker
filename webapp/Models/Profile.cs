using System;
using System.ComponentModel.DataAnnotations;

namespace FriendsAppNoORM.Models
{
    public class Profile
    {
        public long ProfileId { get; set; }
              
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid age")]
        public int Age { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string City { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Interests { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ChangedOn { get; set;}

    }

}