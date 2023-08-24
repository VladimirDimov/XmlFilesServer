namespace Web.Validators
{
    using FluentValidation;
    using Web.Models;

    using static Web.Constants;

    public class FilesUploadModelValidator : AbstractValidator<FilesUploadModel>
    {
        private const string AT_LEAST_ONE_FILE_REQUIRED = "At least one file must be provided";
        private const string INVALID_FILE_TYPE = "Invalid file type: {0}. Only {1} is supported";
        private const string FILE_SIZE_LIMIT = "{0} exceeds the limit of {0}MB";
        private const string FILE_NAME_LIMIT = "File name length must be between {0} and {1}";
        private const string FILE_EXTENSION = "File extension must be .xml";

        public FilesUploadModelValidator(AppSettings settings)
        {
            var fileSettings = settings.FileSettings;

            RuleFor(m => m.Files)
                .Must(files => files?.Any() == true)
                .WithMessage(AT_LEAST_ONE_FILE_REQUIRED);

            RuleForEach(m => m.Files)
                .Must(f => f.ContentType == FileTypes.TEXT_XML)
                .WithMessage((_, f) => string.Format(INVALID_FILE_TYPE, f.ContentType, FileTypes.TEXT_XML));

            RuleForEach(m => m.Files)
                .Must(f =>
                {
                    return f.Length <= fileSettings.MaxFileSizeInMegabytes * 1024 * 1024;
                })
                .WithMessage((_, f) => string.Format(FILE_SIZE_LIMIT, f.FileName, fileSettings.MaxFileSizeInMegabytes));

            RuleForEach(m => m.Files)
                .Must((_, f) =>
                {
                    var extension = Path.GetExtension(f.FileName);

                    return extension == CommonConstants.XML_FILE_EXTENSION;
                })
                .WithMessage(FILE_EXTENSION);

            RuleForEach(m => m.Files)
                .Must((_, f) =>
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(f.FileName);

                    return fileSettings.MinFileNameLength <= fileNameWithoutExtension.Length && fileNameWithoutExtension.Length <= fileSettings.MaxFileNameLength;
                })
                .WithMessage(string.Format(FILE_NAME_LIMIT, fileSettings.MinFileNameLength, fileSettings.MaxFileNameLength));
        }
    }
}
