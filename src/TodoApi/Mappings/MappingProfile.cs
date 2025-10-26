using AutoMapper;
using TodoApi.Core.Entities;
using TodoApi.Core.Entities.Enums;
using TodoApi.DTOs;

namespace TodoApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoTask, TodoTaskDto>()
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));

        CreateMap<CreateTodoTaskDto, TodoTask>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.Priority, o => o.MapFrom(s => Enum.Parse<Priority>(s.Priority, true)))
            .ForMember(d => d.IsCompleted, o => o.MapFrom(_ => false))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.UpdatedAt, o => o.Ignore());


        CreateMap<UpdateTodoTaskDto, TodoTask>()
            .ForMember(d => d.Title, o => o.Condition(src => !string.IsNullOrWhiteSpace(src.Title)))
            .ForMember(d => d.Description, o => o.Condition(src => !string.IsNullOrWhiteSpace(src.Description)))
            .ForMember(d => d.Tags, o => o.Condition(src => !string.IsNullOrWhiteSpace(src.Tags)))
            .ForMember(d => d.IsCompleted, o => o.Condition(src => src.IsCompleted.HasValue))
            .ForMember(d => d.DueDate, o => o.Condition(src => src.DueDate.HasValue))
            .ForMember(d => d.Priority, o => o.Condition(src => src.Priority.HasValue))
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
    }
}