using System;

namespace TheWorkBook.Identity
{
    public static class AccountOptions
    {
        public const string AccountAlreadyExistsErrorMessage = "Account already exists for these details. Please log in.";
        public const bool AllowLocalLogin = true;
        public const bool AllowRememberLogin = true;
        public const bool AutomaticRedirectAfterSignOut = false;
        public const string InvalidCredentialsErrorMessage = "Invalid username or password";
        public const bool ShowLogoutPrompt = true;

        public static TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);
    }
}
