namespace Web.Validators
{
    using FluentValidation;
    using Web.Models;

    using static Web.Constants;

    public class FilesUploadModelValidator : AbstractValidator<FilesUploadModel>
    {
        private const string MAX_NUMBER_OF_FILES = "The number of files excceeds the maximum allowed nomber of {0}";
        private const string AT_LEAST_ONE_FILE_REQUIRED = "At least one file must be provided";
        private const string ONLY_UNIQUE_FILE_NAMES_ALLOWED = "Only unique file names are allowed within a single request. The following files appear more than once: {0}";
        private const string INVALID_FILE_TYPE = "Invalid file type: {0}. Only {1} is supported";
        private const string FILE_SIZE_LIMIT = "{0} exceeds the limit of {1} MB";
        private const string FILE_NAME_LIMIT = "File name length must be between {0} and {1}";
        private const string FILE_EXTENSION = "File extension must be .xml";
        private const string FILE_NAME_CANNOT_BE_NULL = "File name cannot be null";
        private const string INVALID_EMPTY_FILE = "Invalid empty file: {0}";

        public FilesUploadModelValidator(AppSettings settings)
        {
            var fileSettings = settings.FileSettings;

            ClassLevelCascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(m => m.Files)
                .Must(files =>
                {
                    return files?.Any() == true;
                })
                .WithMessage(AT_LEAST_ONE_FILE_REQUIRED);

            RuleForEach(m => m.Files)
                .Must(f => f.Length > 0)
                .WithMessage((_, f) => string.Format(INVALID_EMPTY_FILE, f.FileName));

            RuleFor(m => m.Files)
                .Must(f => f.Count() <= 10)
                .WithMessage((_, f) => string.Format(MAX_NUMBER_OF_FILES, settings.FileSettings.MaxNumberOfFilesPerRequest));

            RuleForEach(m => m.Files)
                .Must((_, f) =>
                {
                    return !string.IsNullOrWhiteSpace(f.FileName);
                })
                .WithMessage(FILE_NAME_CANNOT_BE_NULL);

            RuleFor(m => m.Files)
                .Must(files => !files.GroupBy(f => f.FileName).Any(gr => gr.Count() > 1))
                .WithMessage(m => string.Format(ONLY_UNIQUE_FILE_NAMES_ALLOWED, string.Join(',', m.Files.GroupBy(f => f.FileName).Where(gr => gr.Count() > 1).Select(gr => gr.Key))));

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
                    if (f.FileName is null)
                        return false;

                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(f.FileName);

                    return fileSettings.MinFileNameLength <= fileNameWithoutExtension.Length && fileNameWithoutExtension.Length <= fileSettings.MaxFileNameLength;
                })
                .WithMessage(string.Format(FILE_NAME_LIMIT, fileSettings.MinFileNameLength, fileSettings.MaxFileNameLength));

            RuleForEach(m => m.Files)
                .Must(f => f.ContentType == FileTypes.TEXT_XML)
                .WithMessage((_, f) => string.Format(INVALID_FILE_TYPE, f.ContentType, FileTypes.TEXT_XML));
        }
    }
}
