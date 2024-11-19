using Microsoft.AspNetCore.Mvc;
using PoultryPro_Portal.Services;
using System.Threading.Tasks;

namespace PoultryPro_Portal.Controllers
{
    [Route("Customer")]
    public class CustomerController : Controller
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            //await _customerService.SeedDataAsync();
            return View();
        }

        [HttpGet("GetSuppliers")]
        public async Task<IActionResult> GetSuppliers(int page=1,int itemsPerPage=10)
        {
            try
            {
                var suppliers = await _customerService.GetSuppliersAsync(page,itemsPerPage);
                return Json(suppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

     


        [HttpGet("GetWholesalers")]
        public async Task<IActionResult> GetWholesalers()
        {
            try
            {
                var wholesalers = await _customerService.GetWholesalersAsync();
                return Json(wholesalers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("SearchSuppliers")]
        public async Task<IActionResult> SearchSuppliers(string query, string searchType)
        {
            try
            {
                var suppliers = await _customerService.SearchSuppliersAsync(query, searchType);
                return Json(suppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("SearchWholesalers")]
        public async Task<IActionResult> SearchWholesalers(string query, string searchType)
        {
            try
            {
                var wholesalers = await _customerService.SearchWholesalersAsync(query, searchType);
                return Json(wholesalers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("GetSupplierDetails/{id}")]
        public async Task<IActionResult> GetSupplierDetails(string id)
        {
            try
            {
                var supplier = await _customerService.GetSupplierDetailsAsync(id);
                if (supplier == null)
                {
                    return NotFound();
                }
                return Json(supplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetWholesalerDetails/{id}")]
        public async Task<IActionResult> GetWholesalerDetails(string id)
        {
            try
            {
                var wholesaler = await _customerService.GetWholesalerDetailsAsync(id);
                if (wholesaler == null)
                {
                    return NotFound();
                }
                return Json(wholesaler);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}