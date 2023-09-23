using Real_Estate_Laba.Models;
using System.ComponentModel.DataAnnotations;

namespace Real_Estate_Laba.ViewModels;

public class PropertyVM
{
    public Property Property { get; set; }
    public Location Location { get; set; }
    [Required]
    [Display(Name = "Image File")]
    public IFormFile ImageFile { get; set; }
}
