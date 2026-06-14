using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KenalPhishing.Models
{
    /// <summary>
    /// Tracks one record per user per calendar day.
    /// Used for streak calculation and Konsistensi Latihan grid.
    /// </summary>
    [Table("UserLogins")]
    public class UserLogin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        /// <summary>Stored as DATE only (no time component) e.g. 2025-06-09</summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime LoginDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
