﻿using Microsoft.EntityFrameworkCore;

namespace ChatWeb3.Models
{
    public class User
    {
        public Guid id { get; set; } = Guid.Empty;
        public string accountAddress { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.MinValue;
        public DateTime updatedAt { get; set; } = DateTime.MinValue;
        public bool isDeleted { get; set; } = false;
        public DateTime lastActive { get; set; } = DateTime.Now;
        public bool isOnline { get; set; } = false;
        public string pathToProfilePic { get; set; } = string.Empty;

        public User() { }
        public User(string accountAddress,string username, string pathToProfilePic)
        {
            id = Guid.NewGuid();
            this.accountAddress = accountAddress;
            this.username = username;
            this.pathToProfilePic = pathToProfilePic;
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
            lastActive = DateTime.Now;
            isOnline = false;
            isDeleted = false;
        }

        public User(User user,UpdateUser update)
        {
            this.id = user.id;
            this.accountAddress= user.accountAddress;
            this.username = update.username;
            this.pathToProfilePic= update.pathToProfilePic;
            this.createdAt = user.createdAt;
            this.updatedAt = DateTime.Now;
            this.lastActive = DateTime.Now;
            this.isOnline = user.isOnline;
            this.isDeleted = false;
        }
    }
}

