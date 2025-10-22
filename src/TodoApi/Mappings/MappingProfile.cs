using AutoMapper;
using TodoApi.Core.Entities;
using TodoApi.DTOs;

namespace TodoApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoTask, TodoTaskDto>()
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));

        CreateMap<CreateTodoTaskDto, TodoTask>();
        
        CreateMap<UpdateTodoTaskDto, TodoTask>();
    }
}