// InterviewSystem/Application/Mappings/MappingProfile.cs
using AutoMapper;
using InterviewSystem.Application.DTOs;
using InterviewSystem.Domain.Entities;

namespace InterviewSystem.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : null));
        CreateMap<UserDto, ApplicationUser>()
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.Interviews, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());

        // Company mappings
        CreateMap<Company, CompanyDto>();
        CreateMap<CompanyDto, Company>()
            .ForMember(dest => dest.Users, opt => opt.Ignore())
            .ForMember(dest => dest.Interviews, opt => opt.Ignore());

        // Interview mappings
        CreateMap<Interview, InterviewDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));
        CreateMap<InterviewDto, Interview>()
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.Candidates, opt => opt.Ignore());

        // Candidate mappings
        CreateMap<Candidate, CandidateDto>()
            .ForMember(dest => dest.InterviewTitle, opt => opt.MapFrom(src => src.Interview.Title));
        CreateMap<CandidateDto, Candidate>()
            .ForMember(dest => dest.Interview, opt => opt.Ignore());
    }
}