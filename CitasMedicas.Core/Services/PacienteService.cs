using AutoMapper;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

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
            var pacientes = await _unitOfWork.Paciente.GetAll();
            if (pacientes.Any(p => p.Correo == dto.Correo))
                return null;

            var paciente = _mapper.Map<Paciente>(dto);
            paciente.Saldo = 0;

            await _unitOfWork.Paciente.Add(paciente);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PacienteDto>(paciente);
        }

        public async Task<IEnumerable<PacienteDto>> ObtenerPacientesAsync()
        {
            var pacientes = await _unitOfWork.Paciente.GetAll();
            return _mapper.Map<IEnumerable<PacienteDto>>(pacientes);
        }

        public async Task UpdatePaciente(Paciente paciente)
        {
            await _unitOfWork.Paciente.Update(paciente);
        }

        public async Task DeletePaciente(int id)
        {
            await _unitOfWork.Paciente.Delete(id);
        }
    }
}