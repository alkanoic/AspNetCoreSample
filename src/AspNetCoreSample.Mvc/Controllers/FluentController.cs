using AspNetCoreSample.Mvc.Models;

using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class FluentController : Controller
{
    private readonly ILogger<FluentController> _logger;
    private readonly IValidator<FluentViewModel> _fluentViewModelValidator;

    public FluentController(ILogger<FluentController> logger,
        IValidator<FluentViewModel> fluentViewModelValidator)
    {
        _logger = logger;
        _fluentViewModelValidator = fluentViewModelValidator;
    }

    public IActionResult Index()
    {
        var vm = new FluentViewModel();
        return View(vm);
    }

    [HttpPost]
    public async ValueTask<IActionResult> Index(FluentViewModel vm)
    {
        ValidationResult result = await _fluentViewModelValidator.ValidateAsync(vm);

        if (!result.IsValid)
        {
            result.AddToModelState(this.ModelState);

            return View(vm);
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }
        ModelState.AddModelError("Name", "プロパティに依存するエラー (ModelOnly)");
        ModelState.AddModelError("", "空のキーエラー (ModelOnly)");
        return View(vm);
    }
}
