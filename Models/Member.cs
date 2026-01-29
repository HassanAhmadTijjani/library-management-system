using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models;

public class Member
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime MemberShipDate { get; set; } = DateTime.Now;
    public ICollection<BorrowRecord>? BorrowRecords { get; set; }

}
