using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            return Content(
                file == null
                    ? "FILE = NULL"
                    : $"FILE = {file.FileName}, SIZE = {file.Length}");
        }
    }
}