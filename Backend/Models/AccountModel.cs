using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class AccountModel
{
    [Key]
    public int Id { get; set; }

    public decimal Balance { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    public IdentityUserEx User { get; set; } = null!;
}
