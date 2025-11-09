using AutoMapper;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;

namespace CitasMedicas.Core.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MedicoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MedicoDto?> RegistrarMedicoAsync(MedicoDto dto)
        {
            var medicos = await _unitOfWork.MedicoRepository.GetAll();
            if (medicos.Any(m => m.Correo == dto.Correo))
                return null;

            var medico = _mapper.Map<Medico>(dto);
            await _unitOfWork.MedicoRepository.Add(medico);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MedicoDto>(medico);
        }

        public async Task<IEnumerable<MedicoDto>> ObtenerMedicosAsync()
        {
            var medicos = await _unitOfWork.MedicoRepository.GetAll();
            return _mapper.Map<IEnumerable<MedicoDto>>(medicos);
        }
    }
}
