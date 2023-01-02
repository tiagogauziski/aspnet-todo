using System.Threading;
using TodoList.UI.MVC.TodoApiClient.Contracts;

namespace TodoList.UI.MVC.TodoApiClient
{
    public class TodoApiClient : ITodoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TodoApiClient> _logger;

        public TodoApiClient(HttpClient httpClient, ILogger<TodoApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpClient.BaseAddress = new Uri("https://localhost:7128/api/todoitem");
        }

        public async Task Delete(long id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/{id}", cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<TodoItem>> GetAll(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IEnumerable<TodoItem>>();
        }

        public async Task<TodoItem> GetById(long id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TodoItem>();
        }

        public async Task Post(TodoItem item, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("", item, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task Put(TodoItem item, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("", item, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
