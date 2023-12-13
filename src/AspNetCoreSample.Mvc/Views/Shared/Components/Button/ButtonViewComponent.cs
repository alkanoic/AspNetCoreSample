using AspNetCoreSample.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc;

public class ButtonViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string controlName)
    {
        return View(new ButtonViewModel(controlName));
    }
}
