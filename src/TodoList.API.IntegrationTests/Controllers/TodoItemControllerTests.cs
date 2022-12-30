using Azure.Core;
using TodoList.API.DataTransferObjects;
using TodoList.API.Infrastructure.Database;
using TodoList.API.Models;

namespace TodoList.API.IntegrationTests.Controllers
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
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetTodoItems_ShouldReturn_OK()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/api/todoitem/");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            // Verify payload
            var responseJson = await response.Content.ReadFromJsonAsync<List<TodoItemDto>>();
            Assert.NotNull(responseJson);
        }

        [Fact]
        public async Task GetTodoItem_WhenItemExists_ShouldReturn_OK()
        {
            // Arrange
            var databaseModel = new TodoItem()
            {
                DateAdded = DateTimeOffset.UtcNow,
                IsCompleted = false,
                Name = "test todo get"
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var databaseContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
                await databaseContext.AddAsync(databaseModel);
                await databaseContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync($"/api/todoitem/{databaseModel.Id}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            // Verify payload
            var responseJson = await response.Content.ReadFromJsonAsync<TodoItemDto>();
            Assert.NotNull(responseJson);
            Assert.Equal(databaseModel.Id, responseJson.Id);
            Assert.Equal(databaseModel.Name, responseJson.Name);
            Assert.Equal(databaseModel.IsCompleted, responseJson.IsCompleted);
        }

        [Fact]
        public async Task PutTodoItem_WhenInvalidId_ShouldReturn_NotFound()
        {
            // Arrange
            var request = new TodoItemDto()
            {
                Id = 123,
                IsCompleted = true,
                Name = "invalid todo"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/todoitem/123", request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutTodoItem_WhenRouteIdAndPayloadIdNotInSync_ShouldReturn_BadRequest()
        {
            // Arrange
            var request = new TodoItemDto()
            {
                Id = 123,
                IsCompleted = true,
                Name = "invalid todo"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/todoitem/321", request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutTodoItem_WhenItemExists_ShouldReturn_NoContent()
        {
            // Arrange
            var databaseModel = new TodoItem()
            {
                DateAdded = DateTimeOffset.UtcNow,
                IsCompleted = false,
                Name = "test todo put"
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var databaseContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
                await databaseContext.AddAsync(databaseModel);
                await databaseContext.SaveChangesAsync();
            }

            // Act
            var request = new TodoItemDto()
            {
                Id = databaseModel.Id,
                IsCompleted = !databaseModel.IsCompleted,
                Name = "different name"
            };
            var response = await _client.PutAsJsonAsync($"/api/todoitem/{request.Id}", request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
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
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            // Verify payload
            var responseJson = await response.Content.ReadFromJsonAsync<TodoItemDto>();
            Assert.NotNull(responseJson);
            Assert.Equal(request.Name, responseJson.Name);
            Assert.Equal(request.IsCompleted, responseJson.IsCompleted);

            // Verify Location header
            Assert.NotNull(response.Headers.Location);
            Assert.Contains($"/api/todoitem/{responseJson.Id}", response.Headers.Location.LocalPath);
        }

        [Fact]
        public async Task DeleteTodoItem_WhenInvalidId_ShouldReturn_NotFound()
        {
            // Arrange

            // Act
            var response = await _client.DeleteAsync("/api/todoitem/123");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTodoItem_WhenItemExists_ShouldReturn_NoContent()
        {
            // Arrange
            var databaseModel = new TodoItem()
            {
                DateAdded = DateTimeOffset.UtcNow,
                IsCompleted = false,
                Name = "test todo put"
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var databaseContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
                await databaseContext.AddAsync(databaseModel);
                await databaseContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.DeleteAsync($"/api/todoitem/{databaseModel.Id}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
