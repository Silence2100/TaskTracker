using AutoMapper;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<Project, ProjectDto>()
            .ForMember(
                dest => dest.TasksCount,
                opt => opt.MapFrom(src => src.Tasks.Count))
            .ForMember(
                dest => dest.MembersCount,
                opt => opt.MapFrom(src => src.Members.Count));

        CreateMap<ProjectMember, ProjectMemberDto>()
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src => src.User.Name))
            .ForMember(
                dest => dest.UserEmail,
                opt => opt.MapFrom(src => src.User.Email));

        CreateMap<TaskItem, TaskDto>()
            .ForMember(
                dest => dest.ProjectName,
                opt => opt.MapFrom(src => src.Project.Name))
            .ForMember(
                dest => dest.AssignedUserName,
                opt => opt.MapFrom(src => src.AssignedUser == null ? null : src.AssignedUser.Name))
            .ForMember(
                dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author.Name));
    }
}