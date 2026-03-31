using Repository.DataModels;

namespace Repository.Interfaces
{
    public interface INotificationRepository
    {
        /// <summary>
        /// Create a broadcast notification (visible to all users). Returns the notification id.
        /// </summary>
        Task<long> CreateNotificationAsync(CreateNotificationDTO dto);

        /// <summary>
        /// Create a targeted notification visible only to specific users. Returns the notification id.
        /// </summary>
        Task<long> CreateTargetedNotificationAsync(CreateNotificationDTO dto, IEnumerable<int> targetUserIds);

        /// <summary>
        /// Get notifications for a specific user (not deleted), ordered by newest first.
        /// </summary>
        Task<IEnumerable<NotificationDTO>> GetByUserIdAsync(int userId, int limit = 50);

        /// <summary>
        /// Get unread count for a specific user.
        /// </summary>
        Task<int> GetUnreadCountAsync(int userId);

        /// <summary>
        /// Mark a single notification as read for a user.
        /// </summary>
        Task MarkAsReadAsync(long notificationId, int userId);

        /// <summary>
        /// Mark all notifications as read for a user.
        /// </summary>
        Task MarkAllAsReadAsync(int userId);

        /// <summary>
        /// Soft-delete a notification for a user.
        /// </summary>
        Task DeleteForUserAsync(long notificationId, int userId);
    }
}
