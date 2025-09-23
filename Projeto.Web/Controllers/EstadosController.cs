using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.Web.Controllers
{
    public class EstadosController : Controller
    {
        private readonly IEstadoService _estadoService;

        public EstadosController(IEstadoService estadoService)
        {
            _estadoService = estadoService;
        }

        // GET: Estados
        public async Task<IActionResult> Index()
        {
            var estados = await _estadoService.ObterTodosAsync();
            return View(estados);
        }

        // GET: Estados/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var estado = await _estadoService.ObterPorIdAsync(id);
            if (estado == null)
            {
                return NotFound();
            }

            return View(estado);
        }

        // GET: Estados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,UF")] Estado estado)
        {
            if (ModelState.IsValid)
            {
                await _estadoService.AdicionarAsync(estado);
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        // GET: Estados/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var estado = await _estadoService.ObterPorIdAsync(id);
            if (estado == null)
            {
                return NotFound();
            }
            return View(estado);
        }

        // POST: Estados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,UF")] Estado estado)
        {
            if (id != estado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _estadoService.AtualizarAsync(estado);
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        // GET: Estados/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var estado = await _estadoService.ObterPorIdAsync(id);
            if (estado == null)
            {
                return NotFound();
            }

            return View(estado);
        }

        // POST: Estados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _estadoService.RemoverAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}