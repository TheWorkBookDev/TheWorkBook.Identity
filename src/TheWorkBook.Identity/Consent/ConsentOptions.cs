namespace TheWorkBook.Identity
{
    public static class ConsentOptions
    {
        public static readonly string InvalidSelectionErrorMessage = "Invalid selection";
        public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";
        public const bool EnableOfflineAccess = true;
        public const string OfflineAccessDescription = "Access to your applications and resources, even when you are offline";
        public const string OfflineAccessDisplayName = "Offline Access";
    }
}
