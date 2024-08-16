using System.ComponentModel.DataAnnotations;

namespace Curus.Repository.ViewModels;

public class CommentDTO
{
    [Required]

    public string content { get; set; }
}