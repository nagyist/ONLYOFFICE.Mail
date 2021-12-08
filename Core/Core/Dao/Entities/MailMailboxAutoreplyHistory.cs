﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;
using System;

namespace ASC.Mail.Core.Dao.Entities
{
    public partial class MailMailboxAutoreplyHistory : BaseEntity
    {
        public int IdMailbox { get; set; }
        public int Tenant { get; set; }
        public string SendingEmail { get; set; }
        public DateTime SendingDate { get; set; }

        public override object[] GetKeys() => new object[] { IdMailbox, SendingEmail };
    }

    public static class MailMailboxAutoreplyHistoryExtension
    {
        public static ModelBuilder AddMailMailboxAutoreplyHistory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailMailboxAutoreplyHistory>(entity =>
            {
                entity.ToTable("mail_mailbox_autoreply_history");

                entity.HasKey(e => new { e.IdMailbox, e.SendingEmail })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Tenant)
                    .HasDatabaseName("tenant");

                entity.Property(e => e.IdMailbox)
                    .HasColumnName("id_mailbox")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tenant)
                    .HasColumnName("tenant")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SendingEmail)
                    .HasColumnName("sending_email")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.SendingDate)
                    .HasColumnName("sending_date")
                    .HasColumnType("datetime");
            });

            return modelBuilder;
        }
    }
}