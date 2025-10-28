using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_agparker9.Models;

public class MovieModel
{
    [Key]
    public int MovieId { get; set; }

    [Required] 
    [StringLength(200)]
    public string Title { get; set; } = null!;
    
    [Url]
    [Display(Name = "IMDB Link")]
    [StringLength(500)]
    public string ImdbLink { get; set; } = null!;
    
    [Required]
    [StringLength(100)]  
    public string Genre { get; set; } = null!;
    
    [Display(Name = "Year of Release")]
    public int Year { get; set; }   
    
    [Display(Name = "Poster")]
    public byte[] Poster { get; set; } =  null!;
}