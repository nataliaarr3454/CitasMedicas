using AutoMapper;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using CitasMedicas.Core.CustomEntities;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using CitasMedicas.Core.Enums;

namespace CitasMedicas.Core.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperContext _dapper;

        public MedicoService(IUnitOfWork unitOfWork, IMapper mapper, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapper = dapper;
        }

        public async Task<MedicoDto?> RegistrarMedicoAsync(MedicoDto dto)
        {
            var medicos = await _unitOfWork.Medico.GetAll();
            if (medicos.Any(m => m.Correo == dto.Correo))
                return null;

            var medico = _mapper.Map<Medico>(dto);
            await _unitOfWork.Medico.Add(medico);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MedicoDto>(medico);
        }

        public async Task<IEnumerable<MedicoDto>> ObtenerMedicosAsync()
        {
            var medicos = await _unitOfWork.Medico.GetAll();
            return _mapper.Map<IEnumerable<MedicoDto>>(medicos);
        }

        
        public async Task<ResponseData> Obtenermedicos(MedicoQueryFilter filter)
        {
            var medicos = await _unitOfWork.Medico.GetAll();

            if (filter.MedicoId.HasValue)
                medicos = medicos.Where(m => m.Id == filter.MedicoId.Value);

            if (!string.IsNullOrEmpty(filter.Nombre))
                medicos = medicos.Where(m => m.Nombre.Contains(filter.Nombre, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.Especialidad))
                medicos = medicos.Where(m => m.Especialidad.Contains(filter.Especialidad, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.Telefono))  
                medicos = medicos.Where(m => m.Telefono.Contains(filter.Telefono));

            var pagedList = PagedList<object>.Create(medicos.Cast<object>(), filter.PageNumber, filter.PageSize);

            return new ResponseData
            {
                Messages = new List<Message>
                {
                    new Message
                    {
                        Type = TypeMessage.information.ToString(),
                        Description = "Médicos obtenidos correctamente."
                    }
                },
                Pagination = pagedList,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task UpdateMedico(Medico medico)
        {
            await _unitOfWork.Medico.Update(medico);
        }

        public async Task DeleteMedico(int id)
        {
            await _unitOfWork.Medico.Delete(id);
        }






        public async Task<IEnumerable<MedicoCantidadCitasDto>> GetCantidadCitasPorMedicoAsync()
        {
            var medicos = await _unitOfWork.Medico.GetAll();
            var citas = await _unitOfWork.Cita.GetAll();

            var reporte = medicos.Select(medico => new MedicoCantidadCitasDto
            {
                MedicoId = medico.Id,
                Nombre = medico.Nombre,
                Especialidad = medico.Especialidad,
                CantidadCitas = citas.Count(c => c.MedicoId == medico.Id)
            });

            return reporte.OrderByDescending(r => r.CantidadCitas);
        }
    }
}
