namespace TaskTracker.Web.Models
{
    public class TodoTask
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool IsDone { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
