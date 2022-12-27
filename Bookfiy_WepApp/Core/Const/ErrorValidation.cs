namespace Bookfiy_WepApp.Core.Const
{
    public static class ErrorValidation
    {
        public const string MaxLength = "Length cannot be more than {1} characters";
        public const string MaxLengthPassword = "The {0} must be at least {2} and at max {1} characters long.";
        public const string Dublicated = "Another record with the same {0} is already exists!";
        public const string DublicatedBook = "Book with the same title is already exists with the same author!";
        public const string NotAllowedExtensions = "Only .png , .jpg , .jpeg is allowed!";
        public const string NotAllowedUserName = "Username can only contain letters or digits.";
        public const string maxSize = "File cannot be more than 2 MB";
        public const string date = "Date cannot be in the future!";
        public const string InvalidRange = "{0} Should be between {0} and {1}";
        public const string InvalidPass = "The password and confirmation password do not match.";
        public const string OnlyEnglish = "Only English letters are allowed.";
        public const string OnlyArabic = "Only Arabic letters are allowed.";
        public const string OnlyNumberandLeter = "Only Arabic/English letters or digits are allowed.";
        public const string DenySpecialCharacters = "Special characters are not allowed.";
        public const string PhoneNumber = "Invalid Phone Number.";
    }
}
