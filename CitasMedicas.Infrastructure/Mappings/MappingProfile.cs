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

            CreateMap<Cita, ReservaCitaDto>().ReverseMap();

            CreateMap<Security, SecurityDto>().ReverseMap();
        }
    }
}

