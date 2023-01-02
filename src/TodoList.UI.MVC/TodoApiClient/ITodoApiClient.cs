using TodoList.UI.MVC.TodoApiClient.Contracts;

namespace TodoList.UI.MVC.TodoApiClient
{
    public interface ITodoApiClient
    {
        Task<IEnumerable<TodoItem>> GetAll(CancellationToken cancellationToken = default);
        Task<TodoItem> GetById(long id, CancellationToken cancellationToken = default);
        Task Put(TodoItem item, CancellationToken cancellationToken = default);
        Task Post(TodoItem item, CancellationToken cancellationToken = default);
        Task Delete(long id, CancellationToken cancellationToken = default);
    }
}
