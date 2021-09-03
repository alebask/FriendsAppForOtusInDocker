using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FriendsAppNoORM.Models;

namespace FriendsAppNoORM{
    public class ProfilePaginatedListViewModel : List<Profile> {
        
        public int PageSize { get; private set; }
        public int PageIndex { get; private set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        [RegularExpression("^[A-Za-zА-Яа-я]+$")]
        public string FirstNameFilter { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        [RegularExpression("^[A-Za-zА-Яа-я]+$")]
        public string LastNameFilter { get; set; }
        
        public long TotalPages { get; private set; }
                
        
        public ProfilePaginatedListViewModel(List<Profile> items, long itemCount, int pageIndex, int pageSize)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            
            TotalPages = (long)Math.Ceiling(itemCount / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (this.PageIndex+1 < TotalPages);
            }
        } 
    }
}