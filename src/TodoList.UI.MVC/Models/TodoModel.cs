using System.ComponentModel.DataAnnotations;

namespace TodoList.UI.MVC.Models;

public class TodoModel
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Display(Name = "Is Completed?")]
    public bool IsCompleted { get; set; }
}
