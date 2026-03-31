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

        public async Task<long> CreateNotificationAsync(CreateNotificationDTO dto)
        {
            string sql = @"
                INSERT INTO notifications (type, title, message, link, icon, is_targeted, created_at)
                VALUES (@Type, @Title, @Message, @Link, @Icon, false, NOW())
                RETURNING id;";

            return await _db.ExecuteScalarAsync<long>(sql, dto);
        }

        public async Task<long> CreateTargetedNotificationAsync(CreateNotificationDTO dto, IEnumerable<int> targetUserIds)
        {
            string insertNotification = @"
                INSERT INTO notifications (type, title, message, link, icon, is_targeted, created_at)
                VALUES (@Type, @Title, @Message, @Link, @Icon, true, NOW())
                RETURNING id;";

            var notificationId = await _db.ExecuteScalarAsync<long>(insertNotification, dto);

            // Insert user_notifications rows for each target user
            string insertTargets = @"
                INSERT INTO user_notifications (notification_id, user_id, is_read, is_deleted, created_at)
                VALUES (@NotificationId, @UserId, false, false, NOW())
                ON CONFLICT (notification_id, user_id) DO NOTHING;";

            foreach (var userId in targetUserIds)
            {
                await _db.ExecuteAsync(insertTargets, new { NotificationId = notificationId, UserId = userId });
            }

            return notificationId;
        }

        public async Task<IEnumerable<NotificationDTO>> GetByUserIdAsync(int userId, int limit = 50)
        {
            // Broadcast (is_targeted=false): show to everyone via LEFT JOIN
            // Targeted (is_targeted=true): show only if user has a user_notifications row
            string sql = @"
                SELECT n.id, n.type, n.title, n.message, n.link, n.icon,
                       COALESCE(un.is_read, false) AS IsRead,
                       n.created_at AS CreatedAt
                FROM notifications n
                LEFT JOIN user_notifications un
                    ON un.notification_id = n.id AND un.user_id = @UserId
                WHERE COALESCE(un.is_deleted, false) = false
                  AND (
                    n.is_targeted = false
                    OR (n.is_targeted = true AND un.user_id IS NOT NULL)
                  )
                ORDER BY n.created_at DESC
                LIMIT @Limit;";

            return await _db.QueryAsync<NotificationDTO>(sql, new { UserId = userId, Limit = limit });
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            // Count unread notifications for this user (broadcast + targeted)
            string sql = @"
                SELECT COUNT(1)
                FROM notifications n
                LEFT JOIN user_notifications un
                    ON un.notification_id = n.id AND un.user_id = @UserId
                WHERE COALESCE(un.is_read, false) = false
                  AND COALESCE(un.is_deleted, false) = false
                  AND (
                    n.is_targeted = false
                    OR (n.is_targeted = true AND un.user_id IS NOT NULL)
                  );";

            return await _db.ExecuteScalarAsync<int>(sql, new { UserId = userId });
        }

        public async Task MarkAsReadAsync(long notificationId, int userId)
        {
            // Upsert: insert user_notifications row if not exists, otherwise update
            string sql = @"
                INSERT INTO user_notifications (notification_id, user_id, is_read, is_deleted, read_at, created_at)
                VALUES (@NotificationId, @UserId, true, false, NOW(), NOW())
                ON CONFLICT (notification_id, user_id)
                DO UPDATE SET is_read = true, read_at = NOW();";

            await _db.ExecuteAsync(sql, new { NotificationId = notificationId, UserId = userId });
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            // Insert read entries for all unread notifications visible to this user
            string sql = @"
                INSERT INTO user_notifications (notification_id, user_id, is_read, is_deleted, read_at, created_at)
                SELECT n.id, @UserId, true, false, NOW(), NOW()
                FROM notifications n
                LEFT JOIN user_notifications un
                    ON un.notification_id = n.id AND un.user_id = @UserId
                WHERE (
                    n.is_targeted = false
                    OR (n.is_targeted = true AND un.user_id IS NOT NULL)
                  )
                  AND COALESCE(un.is_read, false) = false
                  AND COALESCE(un.is_deleted, false) = false
                ON CONFLICT (notification_id, user_id)
                DO UPDATE SET is_read = true, read_at = NOW();";

            await _db.ExecuteAsync(sql, new { UserId = userId });
        }

        public async Task DeleteForUserAsync(long notificationId, int userId)
        {
            // Upsert: mark as deleted for this user
            string sql = @"
                INSERT INTO user_notifications (notification_id, user_id, is_read, is_deleted, created_at)
                VALUES (@NotificationId, @UserId, false, true, NOW())
                ON CONFLICT (notification_id, user_id)
                DO UPDATE SET is_deleted = true;";

            await _db.ExecuteAsync(sql, new { NotificationId = notificationId, UserId = userId });
        }
    }
}
