using System.ComponentModel.DataAnnotations;

namespace otoszerelo.Models;

public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A név megadása kötelező.")]
    [StringLength(100, ErrorMessage = "A név legfeljebb 100 karakter lehet.")]
    [Display(Name = "Név")]
    public string Name { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "A telefonszám legfeljebb 20 karakter lehet.")]
    [Display(Name = "Telefonszám")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Adj meg egy érvényes e-mail-címet.")]
    [StringLength(254)]
    [Display(Name = "E-mail-cím")]
    public string? Email { get; set; }

    public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
