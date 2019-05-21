namespace TeaTime.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("ErrorStatusCode")]
        public IActionResult ErrorStatusCode(int code)
        {
            return View(new ErrorViewModel
            {
                StatusCode = code
            });
        }
    }
}