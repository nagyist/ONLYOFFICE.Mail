﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;

namespace ASC.Mail.Core.Dao.Entities
{
    public partial class MailServerServerType : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override object[] GetKeys() => new object[] { Id };
    }

    public static class MailServerServerTypeExtension
    {
        public static ModelBuilder AddMailServerServerType(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailServerServerType>(entity =>
            {
                entity.ToTable("mail_server_server_type");

                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            return modelBuilder;
        }
    }
}