using TodoList.UI.MVC.TodoApiClient.Contracts;

namespace TodoList.UI.MVC.TodoApiClient
{
    public interface ITodoApiClient
    {
        Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TodoItem> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task PutAsync(TodoItem item, CancellationToken cancellationToken = default);
        Task PostAsync(TodoItem item, CancellationToken cancellationToken = default);
        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
        Task IsHealthyAsync(CancellationToken cancellationToken = default);
    }
}
