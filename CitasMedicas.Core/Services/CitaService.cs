using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Exceptions;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Enums;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Services
{
    public class CitaService : ICitaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CitaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllCitasResponseAsync(CitaQueryFilter filters)
        {
            var citas = await _unitOfWork.CitaRepository.GetAll();

            if (filters.PacienteId.HasValue)
                citas = citas.Where(c => c.PacienteId == filters.PacienteId.Value);

            if (filters.MedicoId.HasValue)
                citas = citas.Where(c => c.MedicoId == filters.MedicoId.Value);

            if (!string.IsNullOrEmpty(filters.Estado))
                citas = citas.Where(c => c.Estado.Equals(filters.Estado, StringComparison.OrdinalIgnoreCase));

            if (filters.FechaCita.HasValue)
                citas = citas.Where(c => c.FechaCita.Date == filters.FechaCita.Value.Date);

            var pagedCitas = PagedList<object>.Create(citas.Cast<object>(), filters.PageNumber, filters.PageSize);

            if (pagedCitas.Any())
            {
                return new ResponseData
                {
                    Messages = new[]
                    {
            new Message
            {
                Type = TypeMessage.information.ToString(),
                Description = "Citas recuperadas correctamente."
            }
        },
                    Pagination = pagedCitas,
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new ResponseData
                {
                    Messages = new[]
                    {
            new Message
            {
                Type = TypeMessage.warning.ToString(),
                Description = "No se encontraron citas para los filtros especificados."
            }
        },
                    Pagination = pagedCitas,
                    StatusCode = HttpStatusCode.OK
                };
            }
        }

    public async Task<Cita> ReservarCitaAsync(ReservaCitaDto dto)
        {
            var disponibilidad = await _unitOfWork.DisponibilidadRepository.GetById(dto.DisponibilidadId);
            if (disponibilidad == null)
                throw new BusinessException("La disponibilidad seleccionada no existe.", 404);

            if (disponibilidad.Estado != "Disponible")
                throw new BusinessException("La disponibilidad ya está ocupada.", 400);

             var pacientes = await _unitOfWork.PacienteRepository.GetAll();
            var paciente = pacientes.FirstOrDefault(p =>
                p.Correo.Equals(dto.CorreoPaciente, StringComparison.OrdinalIgnoreCase));
            if (paciente == null)
                throw new BusinessException("El paciente no existe o el correo no está registrado.", 404);

           
            var medico = await _unitOfWork.MedicoRepository.GetById(disponibilidad.MedicoId);
            if (medico == null)
                throw new BusinessException("El médico asociado no existe.", 404);

            var citas = await _unitOfWork.CitaRepository.GetAll();
            var conflicto = citas.Any(c =>
                c.MedicoId == medico.Id &&
                c.FechaCita.Date == disponibilidad.Fecha.Date &&
                c.HoraCita == disponibilidad.HoraInicio);

            if (conflicto)
                throw new BusinessException("El médico ya tiene una cita registrada en ese horario.", 400);

            var cita = new Cita
            {
                PacienteId = paciente.Id,
                MedicoId = medico.Id,
                DisponibilidadId = disponibilidad.Id,
                FechaCita = disponibilidad.Fecha,
                HoraCita = disponibilidad.HoraInicio, 
                Motivo = dto.Motivo,
                Estado = "Reservada",
                FechaRegistro = DateTime.Now
            };

            var pago = new Pago
            {
                Monto = disponibilidad.CostoCita,
                EstadoPago = "Pendiente"
            };
            cita.Pago = pago;

            disponibilidad.Estado = "Ocupado";

           await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.CitaRepository.Add(cita);
                await _unitOfWork.DisponibilidadRepository.Update(disponibilidad);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return cita;
        }

        public async Task<Cita> GetCitaAsync(int id)
        {
            var cita = await _unitOfWork.CitaRepository.GetById(id);
            if (cita == null)
                throw new BusinessException("La cita no existe.", 404);
            return cita;
        }

       
        public async Task UpdateCitaAsync(Cita cita)
        {
            var existente = await _unitOfWork.CitaRepository.GetById(cita.Id);
            if (existente == null)
                throw new BusinessException("La cita no existe.", 404);

            await _unitOfWork.CitaRepository.Update(cita);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCitaAsync(int id)
        {
            var cita = await _unitOfWork.CitaRepository.GetById(id);
            if (cita == null)
                throw new BusinessException("La cita no existe.", 404);

            await _unitOfWork.CitaRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        
        public async Task<IEnumerable<Cita>> GetAllCitasDapperAsync()
        {
            var citas = await _unitOfWork.CitaRepository.GetAll();
            return citas;
        }
    }
}
