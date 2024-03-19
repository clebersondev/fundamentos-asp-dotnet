using System.ComponentModel.DataAnnotations;

namespace BlogEF3.ViewModels.Categories;

public class EditorCategoryViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Esse campo deve conter entre 3 ou 40 caracteres.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O slug é obrigatório.")]
    public string Slug { get; set; }
}