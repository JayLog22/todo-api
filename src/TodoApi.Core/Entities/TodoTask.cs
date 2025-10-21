﻿using TodoApi.Core.Entities.Enums;

namespace TodoApi.Core.Entities;

public class TodoTask : BaseEntity
{
    public string Title { get; set; } =  string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
    public string? Tags { get; set; }
}