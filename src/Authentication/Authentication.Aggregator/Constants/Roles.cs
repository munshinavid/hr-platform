namespace Authentication.Aggregator.Constants
{
    /// <summary>
    /// Role constants for the Authentication bounded context.
    /// Values must match the Role column values in the existing User table.
    /// </summary>
    public static class Roles
    {
        public const string HR = "HR";
        public const string Employee = "Employee";
    }
}
