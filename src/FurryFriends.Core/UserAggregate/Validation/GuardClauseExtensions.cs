using System.Text.RegularExpressions;
using Ardalis.GuardClauses;

namespace FurryFriends.Core.GuardClauses;
    public static class GuardClausesExtensions
    {
        public static void InvalidEmail(this IGuardClause guardClause, string email, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Invalid email address.", parameterName);
            }
        }
    }