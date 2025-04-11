﻿namespace Gallery.Server.Models.User
{
    public class UserModel
    {
        public Guid UserId { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; private set; }


        private UserModel(string username, string passwordHash)
        {
            UserId = Guid.NewGuid(); // Генерируем новый Guid для UserId
            Username = username;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow; // Устанавливаем время создания
            LastLogin = DateTime.UtcNow; // Устанавливаем время последнего входа
        }

        // Статический метод для создания нового пользователя
        public static UserModel CreateUser(string username, string passwordHash)
        {
            return new UserModel(username, passwordHash);
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow; // Обновляем время последнего входа
        }
    }
}