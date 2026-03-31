using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Repository.DataModels;
using Repository.Interfaces;

namespace Bussiness.Services
{
    public interface INotificationService
    {
        Task<long> CreateAndBroadcastAsync(CreateNotification notification);
        Task<IEnumerable<NotificationItem>> GetUserNotificationsAsync(int userId, int limit = 50);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(long notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
        Task DeleteForUserAsync(long notificationId, int userId);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository repo,
            IHubContext<NotificationHub> hubContext,
            IMapper mapper)
        {
            _repo = repo;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task<long> CreateAndBroadcastAsync(CreateNotification notification)
        {
            var dto = _mapper.Map<CreateNotificationDTO>(notification);
            long id;
            bool isTargeted = notification.TargetUserIds != null && notification.TargetUserIds.Count > 0;

            if (isTargeted)
            {
                id = await _repo.CreateTargetedNotificationAsync(dto, notification.TargetUserIds!);
            }
            else
            {
                id = await _repo.CreateNotificationAsync(dto);
            }

            var payload = new
            {
                id,
                type = notification.Type,
                title = notification.Title,
                message = notification.Message,
                link = notification.Link,
                icon = notification.Icon,
                isRead = false,
                createdAt = DateTime.UtcNow
            };

            // If targeted to specific users, send only to those users' connections
            // Otherwise broadcast to all connected clients
            if (isTargeted)
            {
                foreach (var userId in notification.TargetUserIds!)
                {
                    await _hubContext.Clients.User(userId.ToString())
                        .SendAsync("ReceiveNotification", payload);
                }
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", payload);
            }

            return id;
        }

        public async Task<IEnumerable<NotificationItem>> GetUserNotificationsAsync(int userId, int limit = 50)
        {
            var dtos = await _repo.GetByUserIdAsync(userId, limit);
            return _mapper.Map<IEnumerable<NotificationItem>>(dtos);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _repo.GetUnreadCountAsync(userId);
        }

        public async Task MarkAsReadAsync(long notificationId, int userId)
        {
            await _repo.MarkAsReadAsync(notificationId, userId);
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            await _repo.MarkAllAsReadAsync(userId);
        }

        public async Task DeleteForUserAsync(long notificationId, int userId)
        {
            await _repo.DeleteForUserAsync(notificationId, userId);
        }
    }
}
