using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Exceptions;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;

namespace CitasMedicas.Core.Services
{
    public class DisponibilidadService : IDisponibilidadService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DisponibilidadService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Disponibilidad?> RegistrarDisponibilidadAsync(Disponibilidad disponibilidad)
        {
            var lista = await _unitOfWork.DisponibilidadRepository.GetAll();

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

            await _unitOfWork.DisponibilidadRepository.Add(disponibilidad);
            await _unitOfWork.SaveChangesAsync();

            return disponibilidad;
        }

        public async Task<IEnumerable<Disponibilidad>> ObtenerDisponibilidadesPorMedicoAsync(int medicoId)
        {
            var lista = await _unitOfWork.DisponibilidadRepository.GetAll();
            return lista.Where(d => d.MedicoId == medicoId);
        }
       
public async Task<IEnumerable<Disponibilidad>> ObtenerDisponibilidadesFiltradasAsync(DisponibilidadQueryFilter filters)
    {
        var lista = await _unitOfWork.DisponibilidadRepository.GetAll();

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

        return lista;
    }

    }
}
