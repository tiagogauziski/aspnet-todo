namespace TodoList.API.Contracts
{
    public class TodoItemDto
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public bool IsCompleted { get; set; }
    }
}
