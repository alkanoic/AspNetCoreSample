using System.ComponentModel.DataAnnotations;

using FluentValidation;

namespace AspNetCoreSample.Mvc.Models;

public class FluentViewModel
{

    [Display(Name = "お名前")]
    public string? Name { get; set; }

    [Display(Name = "番号")]
    public int No { get; set; }

    [Display(Name = "メールアドレス")]
    public string? Email { get; set; }

    [Display(Name = "オプション")]
    public string? Option { get; set; }
}
public class FluentViewModelValidator : AbstractValidator<FluentViewModel>
{
    public FluentViewModelValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage("{PropertyName}は必須入力ですぅ")
            .Length(1, 5).WithMessage("{MinLength}:{MaxLength}:{PropertyValue}:範囲内の文字数で入力してください");

        RuleFor(x => x.No).LessThan(0).WithMessage("Noは0以上を入力してください");

        RuleFor(x => x.Email).NotNull().WithMessage("Emailは必須入力ですよ")
            .EmailAddress().WithMessage("{PropertyName}:{PropertyValue}:メールアドレスの形式チェック");

        When(x => x.Name == "abc", () =>
        {
            RuleFor(x => x.Option).Length(1, 3).WithMessage("NameがabcのときOptionは3文字以下である必要がある");
        });
    }
}
