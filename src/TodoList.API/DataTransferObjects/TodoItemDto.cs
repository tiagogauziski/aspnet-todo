namespace TodoList.API.DataTransferObjects
{
    public class TodoItemDto
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public bool IsCompleted { get; set; }
    }
}
