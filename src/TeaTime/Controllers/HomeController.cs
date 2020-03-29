namespace TeaTime.Controllers
{
    using System.Threading.Tasks;
    using Common.Features.Statistics.Queries;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var totals = await _mediator.Send(new GetGlobalTotalsQuery());

            var viewModel = new IndexViewModel
            {
                TotalOrdersMade = totals.OrdersMade,
                TotalEndedRuns = totals.RunsEnded
            };

            return View(viewModel);
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