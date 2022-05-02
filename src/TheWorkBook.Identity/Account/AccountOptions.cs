using System;

namespace TheWorkBook.Identity
{
    public class AccountOptions
    {
        public static string AccountAlreadyEsxistsErrorMessage = "Account already exists for these details. Please log in.";
        public static bool AllowLocalLogin = true;
        public static bool AllowRememberLogin = true;
        public static bool AutomaticRedirectAfterSignOut = false;
        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt = true;
    }
}
