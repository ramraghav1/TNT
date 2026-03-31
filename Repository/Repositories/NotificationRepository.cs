using System.Data;
using Dapper;
using Repository.DataModels;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbConnection _db;

        public NotificationRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<long> CreateForAllUsersAsync(CreateNotificationDTO dto)
        {
            // 1. Insert the notification
            string insertNotification = @"
                INSERT INTO notifications (type, title, message, link, icon, created_at)
                VALUES (@Type, @Title, @Message, @Link, @Icon, NOW())
                RETURNING id;";

            var notificationId = await _db.ExecuteScalarAsync<long>(insertNotification, dto);

            // 2. Assign to all active users (from userinformation table)
            string insertUserNotifications = @"
                INSERT INTO user_notifications (notification_id, user_id, is_read, is_deleted, created_at)
                SELECT @NotificationId, userid, false, false, NOW()
                FROM userinformation;";

            await _db.ExecuteAsync(insertUserNotifications, new { NotificationId = notificationId });

            return notificationId;
        }

        public async Task<IEnumerable<NotificationDTO>> GetByUserIdAsync(int userId, int limit = 50)
        {
            string sql = @"
                SELECT n.id, n.type, n.title, n.message, n.link, n.icon,
                       un.is_read AS IsRead, n.created_at AS CreatedAt
                FROM notifications n
                INNER JOIN user_notifications un ON un.notification_id = n.id
                WHERE un.user_id = @UserId AND un.is_deleted = false
                ORDER BY n.created_at DESC
                LIMIT @Limit;";

            return await _db.QueryAsync<NotificationDTO>(sql, new { UserId = userId, Limit = limit });
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            string sql = @"
                SELECT COUNT(1)
                FROM user_notifications
                WHERE user_id = @UserId AND is_read = false AND is_deleted = false;";

            return await _db.ExecuteScalarAsync<int>(sql, new { UserId = userId });
        }

        public async Task MarkAsReadAsync(long notificationId, int userId)
        {
            string sql = @"
                UPDATE user_notifications
                SET is_read = true, read_at = NOW()
                WHERE notification_id = @NotificationId AND user_id = @UserId;";

            await _db.ExecuteAsync(sql, new { NotificationId = notificationId, UserId = userId });
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            string sql = @"
                UPDATE user_notifications
                SET is_read = true, read_at = NOW()
                WHERE user_id = @UserId AND is_read = false;";

            await _db.ExecuteAsync(sql, new { UserId = userId });
        }

        public async Task DeleteForUserAsync(long notificationId, int userId)
        {
            string sql = @"
                UPDATE user_notifications
                SET is_deleted = true
                WHERE notification_id = @NotificationId AND user_id = @UserId;";

            await _db.ExecuteAsync(sql, new { NotificationId = notificationId, UserId = userId });
        }
    }
}
