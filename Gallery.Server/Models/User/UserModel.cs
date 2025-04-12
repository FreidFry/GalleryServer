﻿namespace Gallery.Server.Models.User
{
    public class UserModel
    {
        public Guid UserId { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string AvatarFilePath { get; private set; }


        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; private set; }


        UserModel(string username, string passwordHash)
        {


            UserId = Guid.NewGuid();
            Username = username;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
            LastLogin = DateTime.UtcNow;
            AvatarFilePath = $@"{Environment.CurrentDirectory}/Data/default/img/defaultUserAvatar.png";
        }

        public static UserModel CreateUser(string username, string passwordHash)
        {
            return new UserModel(username, passwordHash);
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }
    }
}