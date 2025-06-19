using AutoMapper;
using FieldBank.Domain.Entities;
using FieldBank.Application.Common.DTOs;
using FieldBank.Application.Features.Fields.Commands.CreateField;
using FieldBank.Application.Features.Fields.Commands.UpdateField;

namespace FieldBank.Application.Common.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Entity to DTO mappings
        CreateMap<Field, FieldDto>();
        
        // Command to Entity mappings
        CreateMap<CreateFieldCommand, Field>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
            
        CreateMap<UpdateFieldCommand, Field>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
    }
} 