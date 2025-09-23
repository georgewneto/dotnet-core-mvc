using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.Web.Controllers
{
    public class CidadesController : Controller
    {
        private readonly ICidadeService _cidadeService;
        private readonly IEstadoService _estadoService;

        public CidadesController(ICidadeService cidadeService, IEstadoService estadoService)
        {
            _cidadeService = cidadeService;
            _estadoService = estadoService;
        }

        // GET: Cidades
        public async Task<IActionResult> Index()
        {
            var cidades = await _cidadeService.ObterTodosAsync();
            return View(cidades);
        }

        // GET: Cidades/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cidade = await _cidadeService.ObterPorIdAsync(id);
            if (cidade == null)
            {
                return NotFound();
            }

            return View(cidade);
        }

        // GET: Cidades/Create
        public async Task<IActionResult> Create()
        {
            await CarregarEstados();
            return View();
        }

        // POST: Cidades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,EstadoId")] Cidade cidade)
        {
            if (ModelState.IsValid)
            {
                await _cidadeService.AdicionarAsync(cidade);
                return RedirectToAction(nameof(Index));
            }
            await CarregarEstados(cidade.EstadoId);
            return View(cidade);
        }

        // GET: Cidades/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cidade = await _cidadeService.ObterPorIdAsync(id);
            if (cidade == null)
            {
                return NotFound();
            }
            await CarregarEstados(cidade.EstadoId);
            return View(cidade);
        }

        // POST: Cidades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,EstadoId")] Cidade cidade)
        {
            if (id != cidade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _cidadeService.AtualizarAsync(cidade);
                return RedirectToAction(nameof(Index));
            }
            await CarregarEstados(cidade.EstadoId);
            return View(cidade);
        }

        // GET: Cidades/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var cidade = await _cidadeService.ObterPorIdAsync(id);
            if (cidade == null)
            {
                return NotFound();
            }

            return View(cidade);
        }

        // POST: Cidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _cidadeService.RemoverAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarEstados(int? estadoId = null)
        {
            var estados = await _estadoService.ObterTodosAsync();
            ViewBag.Estados = new SelectList(estados, "Id", "Nome", estadoId);
        }
    }
}