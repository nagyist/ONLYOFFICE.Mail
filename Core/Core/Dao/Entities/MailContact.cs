﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ASC.Common;
using ASC.Core.Common.EF;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ASC.Mail.Core.Dao.Entities
{
    [Transient]
    [ElasticsearchType(RelationName = Tables.Contact)]
    [Table("mail_contacts")]
    public partial class MailContact : BaseEntity, ISearchItem
    {
        [Key]
        [Column("id", TypeName = "int(11) unsigned")]
        public int Id { get; set; }
        
        [Required]
        [Column("id_user", TypeName = "varchar(255)")]
        public string IdUser { get; set; }
        
        [Column("tenant", TypeName = "int(11)")]
        public int TenantId { get; set; }
        
        [Column("name", TypeName = "varchar(255)")]
        public string Name { get; set; }
        
        [Required]
        [Column("address", TypeName = "varchar(255)")]
        public string Address { get; set; }
        
        [Column("description", TypeName = "varchar(100)")]
        public string Description { get; set; }
        
        [Column("type", TypeName = "int(11)")]
        public int Type { get; set; }
        
        [Column("has_photo")]
        public bool HasPhoto { get; set; }
        
        [Column("last_modified", TypeName = "timestamp")]
        public DateTime LastModified { get; set; }

        [Nested]
        [NotMapped]
        public ICollection<MailContactInfo> InfoList { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get => Tables.Contact;
        }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Name, Address };
        }
    }

    public static class MailContactExtension
    {
        public static ModelBuilder AddMailContact(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailContact>(entity =>
            {
                entity.HasIndex(e => e.LastModified)
                    .HasDatabaseName("last_modified");

                entity.HasIndex(e => new { e.TenantId, e.IdUser, e.Address })
                    .HasDatabaseName("tenant_id_user_name_address");

                entity.Property(e => e.Address)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IdUser)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModified)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Name)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasMany(m => m.InfoList)
                    .WithOne(a => a.Contact)
                    .HasForeignKey(a => a.IdContact);
            });

            return modelBuilder;
        }
    }
}