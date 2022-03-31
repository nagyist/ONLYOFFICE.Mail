﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
namespace ASC.Mail.Core.Dao.Entities;

public partial class MailMailboxProvider : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string DisplayShortName { get; set; }
    public string Documentation { get; set; }

    public override object[] GetKeys() => new object[] { Id };
}

public static class MailMailboxProviderExtension
{
    public static ModelBuilder AddMailMailboxProvider(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MailMailboxProvider>(entity =>
        {
            entity.ToTable("mail_mailbox_provider");

            entity.HasKey(e => e.Id)
                .HasName("PRIMARY");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("int(11)")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DisplayName)
                .HasColumnName("display_name")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.DisplayShortName)
                .HasColumnName("display_short_name")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Documentation)
                .HasColumnName("documentation")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });

        modelBuilder.Entity<MailMailboxProvider>().HasData(new MailMailboxProvider
        {
            Id = 69,
            Name = "googlemail.com",
            DisplayName = "Google Mail",
            DisplayShortName = "GMail"
        });

        return modelBuilder;
    }
}
