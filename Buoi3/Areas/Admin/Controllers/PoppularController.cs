using Buoi3.Models;
using Buoi3.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buoi3.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class PoppularController : Controller
	{
		private readonly IPoppularRepository _poppularRepo;

		public PoppularController(IPoppularRepository poppularRepository)
		{
			_poppularRepo = poppularRepository;
		}

		public async Task<IActionResult> Index()
		{
			var pop = await _poppularRepo.GetAllAsync();
			return View(pop);
		}
		public IActionResult AddAsync()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(Poppular poppular)
		{
			if (ModelState.IsValid)
			{
				await _poppularRepo.AddAsync(poppular);
				return RedirectToAction(nameof(Index));
			}
			return View(poppular);
		}

		public async Task<IActionResult> Display(int id)
		{
			var pop = await _poppularRepo.GetByIdAsync(id);
			if (pop == null)
			{
				return NotFound();
			}
			return View(pop);
		}


		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var pop = await _poppularRepo.GetByIdAsync(id);
			if (pop == null)
			{
				return NotFound();
			}
			return View(pop);
		}

		[HttpPost]
		public async Task<IActionResult> Update(int id, Poppular poppular)
		{
			if (id != poppular.Id)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				await _poppularRepo.UpdateAsync(poppular);
				return RedirectToAction(nameof(Index));
			}
			return View();
		}


		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			var pop = await _poppularRepo.GetByIdAsync(id);
			if (pop == null)
			{
				return NotFound();
			}
			return View(pop);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _poppularRepo.DeleteAsync(id);
			return RedirectToAction("Index");
		}
	}
	
}
