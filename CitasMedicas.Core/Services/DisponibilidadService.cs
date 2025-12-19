using AutoMapper;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Exceptions;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Services
{
    public class DisponibilidadService : IDisponibilidadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperContext _dapper;
        private readonly string[] ForbiddenWords =
        {
            "odio", "violencia", "groseria", "discriminacion"
        };

        public DisponibilidadService(IUnitOfWork unitOfWork, IMapper mapper, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapper = dapper;
        }

        public async Task<Disponibilidad?> RegistrarDisponibilidadAsync(Disponibilidad disponibilidad)
        {
            var lista = await _unitOfWork.Disponibilidad.GetAll();

            var mismasFecha = lista.Where(d =>
                d.MedicoId == disponibilidad.MedicoId &&
                d.Fecha.Date == disponibilidad.Fecha.Date);

            if (mismasFecha.Count() >= 3)
            {
                throw new BusinessException("El médico ya tiene 3 disponibilidades registradas para esta fecha.", 400);
            }

            var solapado = mismasFecha.Any(d =>
                (disponibilidad.HoraInicio < d.HoraFin) && (d.HoraInicio < disponibilidad.HoraFin));

            if (solapado)
                throw new BusinessException("El horario se solapa con otra disponibilidad.", 400);

            await _unitOfWork.Disponibilidad.Add(disponibilidad);
            await _unitOfWork.SaveChangesAsync();

            return disponibilidad;
        }

        public async Task<IEnumerable<Disponibilidad>> ObtenerDisponibilidadesPorMedicoAsync(int medicoId)
        {
            var lista = await _unitOfWork.Disponibilidad.GetAll();
            return lista.Where(d => d.MedicoId == medicoId);
        }

        public async Task<ResponseData> ObtenerDisponibilidadesFiltradasAsync(DisponibilidadQueryFilter filters)
        {
            var lista = await _unitOfWork.Disponibilidad.GetAll();

            if (filters.MedicoId.HasValue)
                lista = lista.Where(d => d.MedicoId == filters.MedicoId.Value);

            if (filters.Fecha.HasValue)
                lista = lista.Where(d => d.Fecha.Date == filters.Fecha.Value.Date);

            if (filters.HoraInicio.HasValue)
                lista = lista.Where(d => d.HoraInicio >= filters.HoraInicio.Value);

            if (filters.HoraFin.HasValue)
                lista = lista.Where(d => d.HoraFin <= filters.HoraFin.Value);

            if (filters.CostoCita.HasValue)
                lista = lista.Where(d => d.CostoCita == filters.CostoCita.Value);

            var pagedList = PagedList<object>.Create(lista.Cast<object>(), filters.PageNumber, filters.PageSize);

            if (pagedList.Any())
            {
                return new ResponseData
                {
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            Type = TypeMessage.information.ToString(),
                            Description = "Disponibilidades obtenidas correctamente."
                        }
                    },
                    Pagination = pagedList,
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new ResponseData
                {
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            Type = TypeMessage.warning.ToString(),
                            Description = "No se encontraron disponibilidades."
                        }
                    },
                    Pagination = pagedList,
                    StatusCode = HttpStatusCode.OK
                };
            }
        }

        public async Task UpdateDisponibilidad(Disponibilidad disponibilidad)
        {
            await _unitOfWork.Disponibilidad.Update(disponibilidad);
        }
    }
}