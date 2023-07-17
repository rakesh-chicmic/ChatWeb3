﻿// <auto-generated />
using System;
using ChatWeb3Frontend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChatWeb3Frontend.Migrations
{
    [DbContext(typeof(ChatAppDbContext))]
    [Migration("20230712103019_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChatWeb3.Models.AccountMessageMapping", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("accountAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("AccountMessagemappings");
                });

            modelBuilder.Entity("ChatWeb3.Models.ChatMappings", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("datetime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("isGroup")
                        .HasColumnType("bit");

                    b.Property<Guid>("receiverId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("senderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("id");

                    b.ToTable("ChatMappings");
                });

            modelBuilder.Entity("ChatWeb3.Models.Group", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("adminId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("noOfParticipants")
                        .HasColumnType("int");

                    b.Property<string>("pathToProfilePic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("updatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("ChatWeb3.Models.Message", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("chatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("isSeen")
                        .HasColumnType("bit");

                    b.Property<string>("pathToFileAttachment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("senderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("ChatWeb3.Models.User", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("accountAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("isOnline")
                        .HasColumnType("bit");

                    b.Property<DateTime>("lastActive")
                        .HasColumnType("datetime2");

                    b.Property<string>("lastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pathToProfilePic")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
