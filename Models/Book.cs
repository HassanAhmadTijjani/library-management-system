using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models;

public class Book
{
    public int Id { get; set; }
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    public string Author { get; set; } =string.Empty;
    [StringLength(13)]
    public string? ISBN { get; set; }
    [Range(1000, 2100)]
    public int PublishedYear { get; set; }
    public bool IsAvailable { get; set; } = true;
    public ICollection<BorrowRecord>? BorrowRecords{ get; set; }

}
