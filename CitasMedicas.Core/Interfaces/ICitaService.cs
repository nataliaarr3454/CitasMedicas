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

    
    }
}
