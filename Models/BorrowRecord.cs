using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models;

public class BorrowRecord
{
     public int Id { get; set; }
        
        // Foreign Keys
        public int BookId { get; set; }
        public int MemberId { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }


        public Book Book { get; set; } = null!;
        public Member Member { get; set; } = null!;

}
