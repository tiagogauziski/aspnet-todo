using Microsoft.Extensions.Options;
using System.Threading;
using TodoList.UI.MVC.TodoApiClient.Contracts;

namespace TodoList.UI.MVC.TodoApiClient
{
    public class TodoApiClient : ITodoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TodoApiClient> _logger;

        public TodoApiClient(HttpClient httpClient, ILogger<TodoApiClient> logger, IOptions<TodoApiOptions> todoApiOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpClient.BaseAddress = todoApiOptions.Value.BaseAddress;
        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/todoitem/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("api/todoitem/", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IEnumerable<TodoItem>>();
        }

        public async Task<TodoItem> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/todoitem/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TodoItem>();
        }

        public async Task PostAsync(TodoItem item, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/todoitem/", item, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task PutAsync(TodoItem item, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/todoitem/{item.Id}", item, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
