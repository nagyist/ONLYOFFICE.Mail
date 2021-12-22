﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ASC.Common;
using ASC.Core.Common.EF;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ASC.Mail.Core.Dao.Entities
{
    public static class Tables
    {
        public const string Mail = "mail_mail";
        public const string Contact = "contact";
        public const string ContactInfo = "contact_info";
        public const string Tag = "tag";
        public const string UserFolder = "user_folder";

    }

    [Transient]
    [ElasticsearchType(RelationName = Tables.Mail)]
    public partial class MailMail : BaseEntity, ISearchItemDocument
    {
        public int Id { get; set; }        
        public int MailboxId { get; set; }
        public string UserId { get; set; } 
        public int TenantId { get; set; } 
        public string Uidl { get; set; }
        public string Md5 { get; set; }
        public string Address { get; set; }
        public string FromText { get; set; }
        public string ToText { get; set; }
        public string ReplyTo { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Introduction { get; set; }
        public bool Importance { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime DateSent { get; set; }
        public int Size { get; set; }
        public int AttachmentsCount { get; set; }
        public bool Unread { get; set; }
        public bool IsAnswered { get; set; }
        public bool IsForwarded { get; set; }
        public bool IsFromCrm { get; set; }
        public bool IsFromTl { get; set; }
        public bool IsTextBodyOnly { get; set; }  
        public bool HasParseError { get; set; }
        public string CalendarUid { get; set; }
        public string Stream { get; set; }
        public int Folder { get; set; }
        public int FolderRestore { get; set; }
        public bool Spam { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public bool IsRemoved { get; set; }
        public string MimeMessageId { get; set; }
        public string MimeInReplyTo { get; set; }
        public string ChainId { get; set; }
        public DateTime ChainDate { get; set; }

        [Nested]
        public List<MailAttachment> Attachments { get; set; }
        
        [Nested]
        public List<MailUserFolder> UserFolders { get; set; }
        
        [Nested]
        public ICollection<MailTag> Tags { get; set; }
        
        public bool HasAttachments { get; set; }
        
        public bool WithCalendar { get; set; }

        public Document Document { get; set; }
       
        [Ignore]
        public string IndexName
        {
            get => Tables.Mail;
        }

        public override object[] GetKeys() => new object[] { Id };

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Subject, FromText, ToText, Cc, Bcc, Document.Attachment.Content };
        }
    }

    public static class MailMailExtension
    {
        public static ModelBuilder AddMailMail(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailMail>(entity =>
            {
                entity.ToTable("mail_mail");

                entity.Ignore(r => r.Document);
                entity.Ignore(r => r.Attachments);
                entity.Ignore(r => r.UserFolders);
                entity.Ignore(r => r.Tags);
                entity.Ignore(r => r.HasAttachments);
                entity.Ignore(r => r.WithCalendar);
                entity.Ignore(r => r.IndexName);

                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => e.LastModifiedOn)
                    .HasDatabaseName("time_modified");

                entity.HasIndex(e => new { e.MailboxId, e.MimeMessageId })
                    .HasDatabaseName("mime_message_id");

                entity.HasIndex(e => new { e.Md5, e.MailboxId })
                    .HasDatabaseName("md5");

                entity.HasIndex(e => new { e.Uidl, e.MailboxId })
                    .HasDatabaseName("uidl");

                entity.HasIndex(e => new { e.ChainId, e.MailboxId, e.Folder })
                    .HasDatabaseName("chain_index_folders");

                entity.HasIndex(e => new { e.TenantId, e.UserId, e.Folder, e.ChainDate })
                    .HasDatabaseName("list_conversations");

                entity.HasIndex(e => new { e.TenantId, e.UserId, e.Folder, e.DateSent })
                    .HasDatabaseName("list_messages");

                entity.Property(e => e.MailboxId)
                    .HasColumnName("id_mailbox")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TenantId)
                    .HasColumnName("tenant")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Bcc)
                    .HasColumnName("bcc")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CalendarUid)
                    .HasColumnName("calendar_uid")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Cc)
                    .HasColumnName("cc")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ChainDate)
                    .HasColumnName("chain_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.ChainId)
                    .HasColumnName("chain_id")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DateReceived)
                    .HasColumnName("date_received")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.DateSent)
                    .HasColumnName("date_sent")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1975-01-01 00:00:00'");


                entity.Property(e => e.FromText)
                    .HasColumnName("from_text")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("id_user")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Introduction)
                    .IsRequired()
                    .HasColumnName("introduction")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Md5)
                .HasColumnName("md5")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MimeInReplyTo)
                    .HasColumnName("mime_in_reply_to")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MimeMessageId)
                    .HasColumnName("mime_message_id")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ReplyTo)
                    .HasColumnName("reply_to")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Stream)
                    .IsRequired()
                    .HasColumnName("stream")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifiedOn)
                    .HasColumnName("time_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ToText)
                    .HasColumnName("to_text")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Uidl)
                    .HasColumnName("uidl")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasMany(m => m.Attachments)
                    .WithOne(a => a.Mail)
                    .HasForeignKey(a => a.IdMail);

                entity.Property(e => e.Importance)
                    .HasColumnName("importance");

                entity.Property(e => e.HasParseError)
                    .HasColumnName("has_parse_error");

                entity.Property(e => e.IsRemoved)
                    .HasColumnName("is_removed");

                entity.Property(e => e.Size)
                    .HasColumnName("size")
                    .HasColumnType("int(11)");

                
                entity.Property(e => e.IsForwarded)
                    .HasColumnName("is_forwarded")
                    .HasColumnType("int(11)");
                
                entity.Property(e => e.IsAnswered)
                    .HasColumnName("is_answered")
                    .HasColumnType("int(11)");
                
                entity.Property(e => e.Unread)
                    .HasColumnName("unread")
                    .HasColumnType("int(11)");
                
                entity.Property(e => e.IsFromCrm)
                    .HasColumnName("is_from_crm")
                    .HasColumnType("int(11)");
                
                entity.Property(e => e.IsFromTl)
                    .HasColumnName("is_from_tl")
                    .HasColumnType("int(11)");
                
                entity.Property(e => e.IsTextBodyOnly)
                    .HasColumnName("is_text_body_only")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AttachmentsCount)
                    .HasColumnName("attachments_count")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Folder)
                    .HasColumnName("folder")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'"); ;
                
                entity.Property(e => e.FolderRestore)
                    .HasColumnName("folder_restore")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'"); ;
                
                entity.Property(e => e.Spam)
                    .HasColumnName("spam")
                    .HasColumnType("int(11)");
            });

            return modelBuilder;
        }
    }
}