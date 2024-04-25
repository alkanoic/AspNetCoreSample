using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace WebApiSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] InputData data)
        {
            var validator = new InputDataValidator();
            var validationResult = validator.Validate(data);

            if (!validationResult.IsValid)
            {
                var errors = new List<ValidationError>();
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(new ValidationError
                    {
                        Field = error.PropertyName,
                        Message = error.ErrorMessage
                    });
                }
                return BadRequest(errors);
            }

            try
            {
                // ここで入力値を処理する
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class InputData
    {
        public string? StringValue { get; set; }
        public int NumberValue { get; set; }
        public DateTime DateValue { get; set; }
    }

    public class ValidationError
    {
        public required string Field { get; set; }
        public required string Message { get; set; }
    }

    public class InputDataValidator : AbstractValidator<InputData>
    {
        public InputDataValidator()
        {
            RuleFor(x => x.StringValue)
                .NotEmpty().WithMessage("文字列は必須です")
                .MaximumLength(100).WithMessage("文字列は100文字以内で入力してください");

            RuleFor(x => x.NumberValue)
                .InclusiveBetween(1, 100).WithMessage("数値は1から100の範囲で入力してください");

            RuleFor(x => x.DateValue)
                .InclusiveBetween(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1))
                .WithMessage("日付は現在から1年前から1年後の範囲で入力してください");
        }
    }
}
