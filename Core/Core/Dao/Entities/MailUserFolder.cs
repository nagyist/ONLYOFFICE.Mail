﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ASC.Core.Common.EF;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Linq.Expressions;

namespace ASC.Mail.Core.Dao.Entities
{
    [ElasticsearchType(RelationName = Tables.UserFolder)]
    public partial class MailUserFolder : BaseEntity, ISearchItem
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int TenantId { get; set; }
        public string IdUser { get; set; }
        public string Name { get; set; }
        public uint FoldersCount { get; set; }
        public uint UnreadMessagesCount { get; set; }
        public uint TotalMessagesCount { get; set; }
        public uint UnreadConversationsCount { get; set; }
        public uint TotalConversationsCount { get; set; }
        public DateTime ModifiedOn { get; set; }

        [Ignore]
        public string IndexName => Tables.UserFolder;

        public override object[] GetKeys() => new object[] { Id };

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Name };
        }
    }

    public static class MailUserFolderExtension
    {
        public static ModelBuilder AddMailUserFolder(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailUserFolder>(entity =>
            {
                entity.ToTable("mail_user_folder");

                entity.Ignore(r => r.IndexName);

                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.TenantId, e.IdUser, e.ParentId })
                    .HasDatabaseName("tenant_user_parent");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parent_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TenantId)
                    .HasColumnName("tenant")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdUser)
                    .IsRequired()
                    .HasColumnName("id_user")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modified_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(400)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FoldersCount)
                    .HasColumnName("folders_count")
                    .HasColumnType("int(11) unsigned");
                
                entity.Property(e => e.UnreadMessagesCount)
                    .HasColumnName("unread_messages_count")
                    .HasColumnType("int(11) unsigned");
                
                entity.Property(e => e.TotalMessagesCount)
                    .HasColumnName("total_messages_count")
                    .HasColumnType("int(11) unsigned");
                
                entity.Property(e => e.UnreadConversationsCount)
                    .HasColumnName("unread_conversations_count")
                    .HasColumnType("int(11) unsigned");
                
                entity.Property(e => e.TotalConversationsCount)
                    .HasColumnName("total_conversations_count")
                    .HasColumnType("int(11) unsigned");
            });

            return modelBuilder;
        }
    }
}