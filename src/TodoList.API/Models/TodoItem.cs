namespace TodoList.API.Models;

public class TodoItem
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public bool IsCompleted { get; set; }

    public DateTimeOffset DateAdded { get; set; }
}
