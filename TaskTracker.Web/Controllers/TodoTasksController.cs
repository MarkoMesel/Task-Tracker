using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;
using TaskTracker.Web.Data;
using TaskTracker.Web.Models;

namespace TaskTracker.Web.Controllers
{
    public class TodoTasksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly KafkaProducerService _kafkaProducer;

        public TodoTasksController(AppDbContext context)
        {
            _context = context;
            _kafkaProducer = new KafkaProducerService();
        }

        // GET: TodoTasks
        public async Task<IActionResult> Index(string filterCondition, string sortOrder)
        {
            var tasks = _context.Tasks.AsQueryable();

            tasks = FilterTasks(tasks, filterCondition);
            tasks = SortTasks(tasks, sortOrder);

            ViewData["filterCondition"] = filterCondition;
            ViewData["sortOrder"] = sortOrder;
            return View(await tasks.ToListAsync());
        }

        private static IQueryable<TodoTask> FilterTasks(IQueryable<TodoTask> tasks, string filterCondition)
        {
            tasks = filterCondition switch
            {
                TaskStatuses.Done => tasks.Where(t => t.IsDone),
                TaskStatuses.Pending => tasks = tasks.Where(t => !t.IsDone),
                _ => tasks
            };

            return tasks;
        }

        private static IQueryable<TodoTask> SortTasks(IQueryable<TodoTask> tasks, string sortOrder)
        {
            tasks = sortOrder switch
            {
                SortOptions.TitleAsc => tasks.OrderBy(t => t.Title),
                SortOptions.TitleDesc => tasks.OrderByDescending(t => t.Title),
                SortOptions.DueDateAsc => tasks
                    .OrderBy(t => t.DueDate.HasValue ? TaskOrder.HasDueDate : TaskOrder.NoDueDate)
                    .ThenBy(t => t.DueDate),                        
                SortOptions.DueDateDesc => tasks
                    .OrderBy(t => t.DueDate.HasValue ? TaskOrder.HasDueDate : TaskOrder.NoDueDate)
                    .ThenByDescending(t => t.DueDate),
                _ => tasks
            };

            return tasks;
        }

        // GET: TodoTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoTask = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoTask == null)
            {
                return NotFound();
            }

            return View(todoTask);
        }

        // GET: TodoTasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TodoTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsDone,DueDate")] TodoTask todoTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todoTask);
                await _context.SaveChangesAsync();
                await _kafkaProducer.SendEventAsync("TaskCreated", new { todoTask.Id, todoTask.Title });
                return RedirectToAction(nameof(Index));
            }
            return View(todoTask);
        }

        // GET: TodoTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoTask = await _context.Tasks.FindAsync(id);
            if (todoTask == null)
            {
                return NotFound();
            }
            return View(todoTask);
        }

        // POST: TodoTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsDone,DueDate")] TodoTask todoTask)
        {
            if (id != todoTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoTaskExists(todoTask.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todoTask);
        }

        // GET: TodoTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoTask = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoTask == null)
            {
                return NotFound();
            }

            return View(todoTask);
        }

        // POST: TodoTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoTask = await _context.Tasks.FindAsync(id);
            if (todoTask != null)
            {
                _context.Tasks.Remove(todoTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoTaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
