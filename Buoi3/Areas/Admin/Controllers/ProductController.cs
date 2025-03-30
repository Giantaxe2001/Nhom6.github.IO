using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Buoi3.Models;
using Buoi3.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace Buoi3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPoppularRepository _poppularRepository;
        private readonly ApplicationDbContext _Context;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IPoppularRepository poppularRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _poppularRepository = poppularRepository;
            _Context = context;
        }
        public class CheckoutViewModel
        {
            public IEnumerable<Product> Products { get; set; }
            public IEnumerable<Category> Categories { get; set; }
        }
        // Hiển thị danh sách sản phẩm
        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("SearchResults", Enumerable.Empty<Product>());
            }

            var products = _Context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .ToList();

            return View("SearchResults", products);
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = categories;
            var poppulars = await _poppularRepository.GetAllAsync();
            ViewBag.poppulars = new SelectList(poppulars, "Id", "Name");
            var products = await _productRepository.GetAllAsync();
            foreach (var product in products)
            {
                if (product.CategoryId != null)
                {
                    product.Category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                }
                if (product.PoppularId != null)
                {
                    product.Poppular = await _poppularRepository.GetByIdAsync(product.PoppularId);
                }
            }
            var viewModel = new CheckoutViewModel
            {
                Products = products,
                Categories = categories
            };
           
            return View(viewModel);
        }
	
		public async Task<IActionResult> ProductCate(int id)
		{
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = categories;
            var poppulars = await _poppularRepository.GetAllAsync();
            ViewBag.poppulars = new SelectList(poppulars, "Id", "Name");
            var products = await _productRepository.GetProductsByCategoryIdAsync(id);
			foreach (var product in products)
			{
				if (product.CategoryId != null)
				{
					product.Category = await _categoryRepository.GetByIdAsync(product.CategoryId);
				}
				if (product.PoppularId != null)
				{
					product.Poppular = await _poppularRepository.GetByIdAsync(product.PoppularId);
				}
			}
            var viewModel = new CheckoutViewModel
            {
                Products = products,
                Categories = categories
            };
            return View(viewModel);
		}
		// Hiển thị form thêm sản phẩm mới
		public async Task<IActionResult> AddAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var poppulars = await _poppularRepository.GetAllAsync();
            ViewBag.poppulars = new SelectList(poppulars, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product product, IFormFile thumbnailImage, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                if (thumbnailImage != null)
                {
                    product.ImageUrl = await SaveImage(thumbnailImage);
                }

                if (imageUrls != null && imageUrls.Any())
                {
                    foreach (var imageUrl in imageUrls)
                    {
                        var savedImageUrl = await SaveImage(imageUrl);
                        product.Images.Add(new ProductImage { Url = savedImageUrl });
                    }
                }

                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, reload categories and poppulars for the form
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var poppulars = await _poppularRepository.GetAllAsync();
            ViewBag.Poppulars = new SelectList(poppulars, "Id", "Name");

            return View(product);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Adjust the path as per your configuration
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Return the relative path
        }





        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var poppulars = await _poppularRepository.GetAllAsync();
            ViewBag.Poppulars = new SelectList(poppulars, "Id", "Name");
            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm
       
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            var poppulars = await _poppularRepository.GetAllAsync();
            ViewBag.poppulars = new SelectList(poppulars, "Id", "Name");
            return View(product);
        }

        [HttpPost]    
        public async Task<IActionResult> Update(int id, Product product, IFormFile thumbnailImage)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (thumbnailImage != null)
                {
                    product.ImageUrl = await SaveImage(thumbnailImage);
                }
                await _productRepository.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }      
            return View(product);
        }


        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
