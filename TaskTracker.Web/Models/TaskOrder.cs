namespace TaskTracker.Models
{
    /// <summary>
    /// Indicates whether a task has a due date. 
    /// Used while sorting tasks to move tasks without a due date to the end of the list.
    /// </summary>
    public static class TaskOrder
    {
        public const int HasDueDate = 0;
        public const int NoDueDate = 1;
    }
}
