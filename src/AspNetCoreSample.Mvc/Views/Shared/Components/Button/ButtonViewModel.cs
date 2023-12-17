namespace AspNetCoreSample.Mvc.Models;

public class ButtonViewModel
{
    public string ControlName { get; set; }

    public ButtonViewModel(string controlName)
    {
        this.ControlName = controlName;
    }
}
