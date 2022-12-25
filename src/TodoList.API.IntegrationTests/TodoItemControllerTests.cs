using Microsoft.AspNetCore.Mvc.Testing;
using TodoList.API.Contracts;
using TodoList.API.Infrastructure.Database;

namespace TodoList.API.IntegrationTests
{
    public class TodoItemControllerTests :
        IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public TodoItemControllerTests(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;

            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var databaseContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
                databaseContext.Database.EnsureCreated();
            }
        }

        [Fact]
        public async Task GetTodoItem_WhenInvalidId_ShouldReturn_NotFound()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/todoitem/123");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostTodoItem_ShouldReturn_Created()
        {
            TodoItemDto request = new TodoItemDto()
            {
                Name = "test task",
                IsCompleted = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/todoitem/", request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            // Verify payload
            var responseJson = await response.Content.ReadFromJsonAsync<TodoItemDto>();
            Assert.NotNull(responseJson);
            Assert.Equal(request.Name, responseJson.Name);
            Assert.Equal(request.IsCompleted, responseJson.IsCompleted);

            // Verify Location header
            Assert.NotNull(response.Headers.Location);
            Assert.Contains("/api/todoitem/1", response.Headers.Location.LocalPath);
        }
    }
}
