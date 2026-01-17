using System.Security.Claims;


namespace ChineseAuction.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // שליפת ID של משתמש
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(id, out var userId) ? userId : 0;
        }

        // שליפת התפקיד של המשתמש
        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }

        // בדיקה אם המשתמש הוא מנהל
        public static bool IsManager(this ClaimsPrincipal user)
        {
            return GetUserRole(user) == "Manager";
        }

    }
 }

