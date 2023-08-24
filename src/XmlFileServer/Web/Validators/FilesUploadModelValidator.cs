namespace Web.Validators
{
    using FluentValidation;
    using Web.Models;

    using static Web.Constants;

    public class FilesUploadModelValidator : AbstractValidator<FilesUploadModel>
    {
        public FilesUploadModelValidator()
        {
            RuleFor(m => m.Files)
                .Must(files => files?.Any() == true)
                .WithMessage("At least one file must be provided");

            RuleForEach(m => m.Files)
                .Must(f => f.ContentType == FileTypes.TextXml)
                .WithMessage((_, f) => $"Invalid file type: {f.ContentType}. Only {FileTypes.TextXml} is supported");

            RuleForEach(m => m.Files)
                .Must(f => f.Length <= 1 * 1024 * 1024)
                .WithMessage((_, f) => $"{f.FileName} exceeds the limit of 1MB");

            RuleForEach(m => m.Files)
                .Must((_, f) =>
                {
                    var extension = Path.GetExtension(f.FileName);

                    return extension == ".xml";
                })
                .WithMessage($"File name length must be between {4} and {30}");

            RuleForEach(m => m.Files)
                .Must((_, f) =>
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(f.FileName);

                    return 1 <= fileNameWithoutExtension.Length && fileNameWithoutExtension.Length <= 20;
                })
                .WithMessage($"File name length must be between {4} and {30}");
        }
    }
}
