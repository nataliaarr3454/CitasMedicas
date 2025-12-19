using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.QueryFilters;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitasMedicas.Core.CustomEntities;

namespace CitasMedicas.Core.Interfaces
{
    public interface ICitaService
    {
        /// <summary>
        /// Obtiene citas aplicando filtros opcionales.
        /// </summary>
        Task<ResponseData> GetAllCitasResponseAsync(CitaQueryFilter filters);
        /// <summary>
        /// Reserva una nueva cita a partir del DTO recibido.
        /// </summary>
        Task<Cita> ReservarCitaAsync(ReservaCitaDto dto);

        /// <summary>
        /// Obtiene una cita por su Id.
        /// </summary>
        Task<Cita> GetCitaAsync(int id);

        /// <summary>
        /// Actualiza una cita existente.
        /// </summary>
        Task UpdateCitaAsync(Cita cita);

        /// <summary>
        /// Elimina una cita existente.
        /// </summary>
        Task DeleteCitaAsync(int id);

        /// <summary>
        /// Obtiene todas las citas vía Dapper (opcional).
        /// </summary>
        Task<IEnumerable<Cita>> GetAllCitasDapperAsync();

        ///<sumary>
        /// Cancela una cita solo con el id de la cita si la cancelacion es antes de las 24 horas
        /// y se reeembolsa el dinero o se cancela el pago
        /// y si es menos de las 24 horas no se cancela la cita
        /// </sumary>
        Task<Cita> CancelarCitaAsync(int id, string? motivoCancelacion = null);
    }
}
