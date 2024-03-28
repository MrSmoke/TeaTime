namespace TeaTime.Controllers;

using System.Threading.Tasks;
using Common.Features.Statistics.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

public class HomeController(ISender mediator) : Controller
{
    [Route(""), HttpGet, HttpHead]
    public async Task<IActionResult> Index()
    {
        var totals = await mediator.Send(new GetGlobalTotalsQuery());

        var viewModel = new IndexViewModel
        {
            TotalOrdersMade = totals.OrdersMade,
            TotalEndedRuns = totals.RunsEnded
        };

        return View(viewModel);
    }

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("contact")]
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
