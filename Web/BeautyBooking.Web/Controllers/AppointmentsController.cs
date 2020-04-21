﻿namespace BeautyBooking.Web.Controllers
{
    using System.Threading.Tasks;

    using BeautyBooking.Data.Models;
    using BeautyBooking.Services.Data.Appointments;
    using BeautyBooking.Services.Data.SalonServicesServices;
    using BeautyBooking.Web.ViewModels.Appointments;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class AppointmentsController : BaseController
    {
        private readonly IAppointmentsService appointmentsService;
        private readonly ISalonServicesService salonServicesService;
        private readonly UserManager<ApplicationUser> userManager;

        public AppointmentsController(
            IAppointmentsService appointmentsService,
            ISalonServicesService salonServicesService,
            UserManager<ApplicationUser> userManager)
        {
            this.appointmentsService = appointmentsService;
            this.salonServicesService = salonServicesService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await this.userManager.GetUserAsync(this.HttpContext.User);
            var userId = await this.userManager.GetUserIdAsync(user);

            var viewModel = new AppointmentsListViewModel
            {
                Appointments = await this.appointmentsService.GetAllByUserAsync<AppointmentViewModel>(userId),
            };
            return this.View(viewModel);
        }

        public async Task<IActionResult> MakeAnAppointment(int salon, int service)
        {
            var salonService = await this.salonServicesService.GetByIdAsync<AppointmentSalonServiceViewModel>(salon, service);
            var viewModel = new AppointmentInputModel
            {
                SalonId = salonService.SalonId,
                SalonName = salonService.SalonName,
                SalonAddress = salonService.SalonAddress,
                ServiceId = salonService.ServiceId,
                ServiceName = salonService.ServiceName,
            };
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MakeAnAppointment(AppointmentInputModel input)
        {
            var user = await this.userManager.GetUserAsync(this.HttpContext.User);
            var userId = await this.userManager.GetUserIdAsync(user);

            await this.appointmentsService.AddAsync(userId, input.SalonId, input.ServiceId, input.Date, input.Time);

            return this.RedirectToAction("Index");
        }
    }
}
