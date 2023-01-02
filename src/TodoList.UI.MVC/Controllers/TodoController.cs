using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TodoList.UI.MVC.Models;
using TodoList.UI.MVC.TodoApiClient;
using TodoList.UI.MVC.TodoApiClient.Contracts;

namespace TodoList.UI.MVC.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoApiClient _todoApiClient;

        public TodoController(ITodoApiClient todoApiClient)
        {
            _todoApiClient = todoApiClient;
        }

        // GET: TodoItemController
        public async Task<ActionResult> Index()
        {
            var todoItems = await _todoApiClient.GetAll();

            return View(todoItems
                .Select(todoItem => new TodoModel() { Id = todoItem.Id, Name = todoItem.Name, IsCompleted = todoItem.IsCompleted })
                .ToList());
        }

        // GET: TodoItemController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var todoItem = await _todoApiClient.GetById(id);

            return View(new TodoModel() { Id = todoItem.Id, Name = todoItem.Name, IsCompleted = todoItem.IsCompleted });
        }

        // GET: TodoItemController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TodoItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Name", "IsCompleted")] TodoModel model)
        {
            await _todoApiClient.Put(new TodoItem(0, model.Name, model.IsCompleted));

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TodoItemController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var todoItem = await _todoApiClient.GetById(id);

            return View(new TodoModel() { Id = todoItem.Id, Name = todoItem.Name, IsCompleted = todoItem.IsCompleted });
        }

        // POST: TodoItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("Name", "IsCompleted")] TodoModel model)
        {
            await _todoApiClient.Put(new TodoItem(id, model.Name, model.IsCompleted));

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TodoItemController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TodoItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            await _todoApiClient.Delete(id);

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
