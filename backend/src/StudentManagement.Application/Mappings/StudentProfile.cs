using AutoMapper;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Mappings;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentDto>();

        CreateMap<CreateStudentDto, Student>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());
    }
}
