using AutoMapper;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Exceptions;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Services
{
    public class CitaService : ICitaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperContext _dapper;

        private readonly string[] ForbiddenWords =
        {
            "odio", "violencia", "groseria", "discriminacion", "pornografia"
        };

        public CitaService(IUnitOfWork unitOfWork, IMapper mapper, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapper = dapper;
        }

        public async Task<ResponseData> GetAllCitasResponseAsync(CitaQueryFilter filters)
        {
            var citas = await _unitOfWork.Cita.GetAll();

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
                    Messages = new List<Message>
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
                    Messages = new List<Message>
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
            var disponibilidad = await _unitOfWork.Disponibilidad.GetById(dto.DisponibilidadId);

            // Primero verifica si la disponibilidad es null
            if (disponibilidad == null)
                throw new BusinessException("La disponibilidad seleccionada no existe.", 404);

            if (disponibilidad.Estado != "Disponible")
                throw new BusinessException("La disponibilidad ya está ocupada.", 400);

            var pacientes = await _unitOfWork.Paciente.GetAll();
            var paciente = pacientes.FirstOrDefault(p =>
                p.Correo.Equals(dto.CorreoPaciente, StringComparison.OrdinalIgnoreCase));

            if (paciente == null)
                throw new BusinessException("El paciente no existe o el correo no está registrado.", 404);

            var medico = await _unitOfWork.Medico.GetById(disponibilidad.MedicoId);
            if (medico == null)
                throw new BusinessException("El médico asociado no existe.", 404);

            var citas = await _unitOfWork.Cita.GetAll();
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
                await _unitOfWork.Cita.Add(cita);
                await _unitOfWork.Disponibilidad.Update(disponibilidad);
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
            var cita = await _unitOfWork.Cita.GetById(id);
            if (cita == null)
                throw new BusinessException("La cita no existe.", 404);
            return cita;
        }

       
        public async Task UpdateCitaAsync(Cita cita)
        {
            var existente = await _unitOfWork.Cita.GetById(cita.Id);
            if (existente == null)
                throw new BusinessException("La cita no existe.", 404);

            await _unitOfWork.Cita.Update(cita);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCitaAsync(int id)
        {
            var cita = await _unitOfWork.Cita.GetById(id);
            if (cita == null)
                throw new BusinessException("La cita no existe.", 404);

            await _unitOfWork.Cita.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        
        public async Task<IEnumerable<Cita>> GetAllCitasDapperAsync()
        {
            var citas = await _unitOfWork.Cita.GetAll();
            return citas;
        }

        public async Task<Cita> CancelarCitaAsync(int id, string? motivoCancelacion = null)
        {
            
            var cita = await _unitOfWork.Cita.GetById(id);
            if (cita == null)
                throw new BusinessException("La cita no existe.", 404);

            
            if (cita.Estado == "Cancelada")
                throw new BusinessException("La cita ya está cancelada.", 400);

            if (cita.Estado == "Completada")
                throw new BusinessException("No se puede cancelar una cita ya completada.", 400);

            
            var fechaHoraCita = cita.FechaCita.Date.Add(cita.HoraCita);
            var tiempoRestante = fechaHoraCita - DateTime.Now;

            
            if (tiempoRestante <= TimeSpan.FromHours(24))
            {
                throw new BusinessException(
                    "No se puede cancelar la cita. Debe cancelarse con al menos 24 horas de anticipación.",
                    400
                );
            }

            
            var disponibilidad = await _unitOfWork.Disponibilidad.GetById(cita.DisponibilidadId);
            if (disponibilidad == null)
                throw new BusinessException("La disponibilidad asociada no existe.", 404);

            
            var pagos = await _unitOfWork.Pago.GetAll();
            var pago = pagos.FirstOrDefault(p => p.CitaId == cita.Id);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                
                cita.Estado = "Cancelada";
                if (!string.IsNullOrEmpty(motivoCancelacion))
                {
                    cita.Motivo = $"{cita.Motivo} (Cancelada: {motivoCancelacion})";
                }

                
                disponibilidad.Estado = "Disponible";

               
                if (pago != null && pago.EstadoPago == "Pendiente")
                {
                    pago.EstadoPago = "Cancelado";
                    await _unitOfWork.Pago.Update(pago);
                }

                
                if (pago != null && pago.EstadoPago == "Pagado")
                {
                    var pacientes = await _unitOfWork.Paciente.GetAll();
                    var paciente = pacientes.FirstOrDefault(p => p.Id == cita.PacienteId);
                    if (paciente != null)
                    {
                        paciente.Saldo += pago.Monto;
                        pago.EstadoPago = "Reembolsado";
                        await _unitOfWork.Paciente.Update(paciente);
                        await _unitOfWork.Pago.Update(pago);
                    }
                }

                await _unitOfWork.Cita.Update(cita);
                await _unitOfWork.Disponibilidad.Update(disponibilidad);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return cita;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
