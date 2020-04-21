﻿namespace BeautyBooking.Services.Data.Appointments
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BeautyBooking.Data.Common.Repositories;
    using BeautyBooking.Data.Models;
    using BeautyBooking.Services.DateTimeParser;
    using BeautyBooking.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class AppointmentsService : IAppointmentsService
    {
        private readonly IDeletableEntityRepository<Appointment> appointmentsRepository;
        private readonly IDateTimeParserService dateTimeParserService;

        public AppointmentsService(
            IDeletableEntityRepository<Appointment> appointmentsRepository,
            IDateTimeParserService dateTimeParserService)
        {
            this.appointmentsRepository = appointmentsRepository;
            this.dateTimeParserService = dateTimeParserService;
        }

        public async Task<int> GetCountAsync()
        {
            return await this.appointmentsRepository.All().CountAsync();
        }

        public async Task<IEnumerable<T>> GetAllByUserAsync<T>(string userId)
        {
            var appointments = await this.appointmentsRepository.All()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Time)
                .To<T>().ToListAsync();
            return appointments;
        }

        public async Task AddAsync(string userId, int salonId, int serviceId, string date, string time)
        {
            var dateTime = this.dateTimeParserService.ConvertString(date, time);

            await this.appointmentsRepository.AddAsync(new Appointment
            {
                Time = dateTime,
                UserId = userId,
                SalonId = salonId,
                ServiceId = serviceId,
            });

            await this.appointmentsRepository.SaveChangesAsync();
        }
    }
}
