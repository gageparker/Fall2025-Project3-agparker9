using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_agparker9.Models;

public class ActorModel
{
    [Key]
    public int ActorId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Gender { get; set; } = null!;
    
    public int Age { get; set; }

    [Url]
    [Display(Name = "IMDB Link")]
    [StringLength(500)]
    public string ImdbLink { get; set; } = null!;

    [Display(Name = "Photo")]
    public byte[] Photo { get; set; } = null!;
}