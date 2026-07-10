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
        CreateMap<User, UserDto>()
            .ForMember(
                destination => destination.Login,
                options => options.MapFrom(source => source.Login.Value))
            .ForMember(
                destination => destination.Email,
                options => options.MapFrom(source => source.Email.Value));

        CreateMap<Project, ProjectDto>()
            .ForMember(
                destination => destination.TasksCount,
                options => options.MapFrom(source => source.Tasks.Count))
            .ForMember(
                destination => destination.MembersCount,
                options => options.MapFrom(source => source.Members.Count));

        CreateMap<ProjectMember, ProjectMemberDto>()
            .ForMember(
                destination => destination.UserName,
                options => options.MapFrom(source => source.User.Name))
            .ForMember(
                destination => destination.UserEmail,
                options => options.MapFrom(source => source.User.Email.Value));

        CreateMap<TaskItem, TaskDto>()
            .ForMember(
                destination => destination.ProjectName,
                options => options.MapFrom(source => source.Project.Name))
            .ForMember(
                destination => destination.AssignedUserName,
                options => options.MapFrom(source => source.AssignedUser == null ? null : source.AssignedUser.Name))
            .ForMember(
                destination => destination.AuthorName,
                options => options.MapFrom(source => source.Author.Name));
    }
}