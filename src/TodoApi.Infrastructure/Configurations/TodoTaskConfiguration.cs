using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApi.Core.Entities;
using TodoApi.Core.Entities.Enums;

namespace TodoApi.Infrastructure.Configurations;

public class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
{

    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        builder.ToTable("TodoTasks");

        #region Keys and indexes

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.IsCompleted);
        builder.HasIndex(x => x.Priority);
        builder.HasIndex(x => x.CreatedAt);
        
        #endregion

        #region Entity base properties

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt);
        
        #endregion
        
        #region Properties
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DueDate);

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasDefaultValue(Priority.Medium);

        builder.Property(x => x.Tags)
            .HasMaxLength(500);
        
        #endregion
    }
}