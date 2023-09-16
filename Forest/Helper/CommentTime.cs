namespace Forest.Helper
{
    public static class CommentTime
    {
        public static string GetTimeAgo(DateTime commentDateTime)
        {
            TimeSpan timeSinceComment = DateTime.Now - commentDateTime;

            if (timeSinceComment.TotalSeconds < 60) return $"{timeSinceComment.Seconds} seconds ago";
            else if (timeSinceComment.TotalMinutes < 60) return $"{timeSinceComment.Minutes} minutes ago";
            else if (timeSinceComment.TotalHours < 24) return $"{timeSinceComment.Hours} hours ago";
            else if (timeSinceComment.TotalDays < 7) return $"{timeSinceComment.Days} day ago";
            else if (timeSinceComment.TotalDays < 30) return $"{timeSinceComment.Days / 7} weeks ago";
            else if (timeSinceComment.TotalDays < 365) return $"{timeSinceComment.Days / 30} months ago";
            else return commentDateTime.ToString("dd/MM/yyyy");
        }
    }
}