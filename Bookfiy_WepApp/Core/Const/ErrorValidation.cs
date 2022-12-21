namespace Bookfiy_WepApp.Core.Const
{
    public static class ErrorValidation
    {
        public const string MaxLength = "Length cannot be more than {1} characters";
        public const string MaxLengthPassword = "The {0} must be at least {2} and at max {1} characters long.";
        public const string Dublicated = "{0} with the same name is already exists!";
        public const string DublicatedBook = "Book with the same title is already exists with the same author!";
        public const string NotAllowedExtensions = "Only .png , .jpg , .jpeg is allowed!";
        public const string maxSize = "File cannot be more than 2 MB";
        public const string date = "Date cannot be in the future!";
        public const string InvalidRange = "{0} Should be between {0} and {1}";
        public const string InvalidPass = "The password and confirmation password do not match.";
    }
}
