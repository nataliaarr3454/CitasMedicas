using AutoMapper;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;

namespace CitasMedicas.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Medico, MedicoDto>().ReverseMap();

            CreateMap<Paciente, PacienteDto>().ReverseMap();

            CreateMap<Disponibilidad, Disponibilidad>().ReverseMap();

            CreateMap<Cita, ReservaCitaDto>()
                .ForMember(dest => dest.CorreoPaciente, opt => opt.MapFrom(src => src.Paciente.Correo))
                .ForMember(dest => dest.Motivo, opt => opt.MapFrom(src => src.Motivo))
                .ForMember(dest => dest.DisponibilidadId, opt => opt.MapFrom(src => src.DisponibilidadId));

            CreateMap<Security, SecurityDto>().ReverseMap();
        }
    }
}

