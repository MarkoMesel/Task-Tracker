using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Web.Models
{
    public class TodoTask
    {
        public int Id { get; set; }
        [Required(ErrorMessage= "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public required string Title { get; set; }
        [StringLength(500, ErrorMessage = "Title cannot be longer than 500 characters")]
        public string? Description { get; set; }
        public bool IsDone { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
    }
}
