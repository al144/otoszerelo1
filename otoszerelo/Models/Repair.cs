using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace otoszerelo.Models;

public enum RepairStatus
{
    [Display(Name = "Felvéve")]
    Received,

    [Display(Name = "Folyamatban")]
    InProgress,

    [Display(Name = "Kész")]
    Completed
}

public class Repair
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Ügyfél")]
    public int CustomerId { get; set; }

    [ValidateNever]
    public Customer Customer { get; set; } = null!;

    [Required(ErrorMessage = "A rendszám megadása kötelező.")]
    [StringLength(10)]
    [RegularExpression(@"^[A-Z]{3}-[0-9]{3}$",
        ErrorMessage = "A rendszám formátuma: ABC-123.")]
    [Display(Name = "Rendszám")]
    public string LicensePlate { get; set; } = string.Empty;

    [Required(ErrorMessage = "A jármű megadása kötelező.")]
    [StringLength(100, ErrorMessage = "A jármű legfeljebb 100 karakter lehet.")]
    [Display(Name = "Jármű")]
    public string Vehicle { get; set; } = string.Empty;

    [Required(ErrorMessage = "A javítás leírása kötelező.")]
    [StringLength(500, ErrorMessage = "A leírás legfeljebb 500 karakter lehet.")]
    [Display(Name = "Javítás leírása")]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999", ParseLimitsInInvariantCulture = true,
        ErrorMessage = "A költségnek nullánál nagyobbnak kell lennie.")]
    [Display(Name = "Költség (Ft)")]
    public decimal? Cost { get; set; }

    [Display(Name = "Állapot")]
    public RepairStatus Status { get; set; } = RepairStatus.Received;

    [Display(Name = "Felvétel dátuma")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public string StatusName => Status switch
    {
        RepairStatus.Received => "Felvéve",
        RepairStatus.InProgress => "Folyamatban",
        RepairStatus.Completed => "Kész",
        _ => "Ismeretlen"
    };
}
