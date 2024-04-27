
using AspNetCoreSample.WebApi.EfModels;

using FluentValidation;

namespace AspNetCoreSample.WebApi.Validators;

public class NameValidator : AbstractValidator<Name>
{
    public NameValidator()
    {
        RuleFor(x => x.Name1)
            .NotEmpty().WithMessage("文字列は必須です")
            .MaximumLength(100).WithMessage("文字列は100文字以内で入力してください");
    }
}
