using FluentValidation;
using Web.Models;
using static Web.Constants;

namespace Web.Validators
{
    public class GetFileModelValidator : AbstractValidator<GetFileModel>
    {
        private const string FILE_EXTENSION = "File extension must be .json";

        public GetFileModelValidator()
        {
            RuleFor(m => m.FileName)
                .NotEmpty()
                .NotNull();

            RuleFor(m => m.FileName)
                .Must(fileName =>
                {
                    var extension = Path.GetExtension(fileName);

                    return extension == CommonConstants.JSON_FILE_EXTENSION;
                })
                .WithMessage(FILE_EXTENSION);
        }
    }
}
