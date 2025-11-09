using AutoMapper;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;

namespace CitasMedicas.Core.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PacienteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PacienteDto?> RegistrarPacienteAsync(PacienteDto dto)
        {
            var pacientes = await _unitOfWork.PacienteRepository.GetAll();
            if (pacientes.Any(p => p.Correo == dto.Correo))
                return null;

            var paciente = _mapper.Map<Paciente>(dto);
            paciente.Saldo = 0;

            await _unitOfWork.PacienteRepository.Add(paciente);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PacienteDto>(paciente);
        }

        public async Task<IEnumerable<PacienteDto>> ObtenerPacientesAsync()
        {
            var pacientes = await _unitOfWork.PacienteRepository.GetAll();
            return _mapper.Map<IEnumerable<PacienteDto>>(pacientes);
        }
    }
}
