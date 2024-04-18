using Mango.Mango.Web.Models;
using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new List<ProductDto>();
            ResponseDto? response =await _productService.GetAllProductsAsync();
            if (response!=null && response.IsSuccess)
            {
                list=JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Messege;
            }
            return View(list);
        } 
        
               

        public  IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ProductDto model = new ProductDto();
                ResponseDto? response = await _productService.CreateProductsAsync(productDto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Messege;
                }
            }
            return View(productDto);
        }

        public async Task<IActionResult> Delete(int productId)
        {
            ResponseDto? result = await _productService.DeleteProductsAsync(productId);
            if (result != null && result.IsSuccess)
            {
                TempData["success"] = "Product Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = result?.Messege;
            }
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Update(int productId)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Messege;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateProductsAsync(productDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Messege;
                }
            }
            return View(productDto);
        }
    }
}
