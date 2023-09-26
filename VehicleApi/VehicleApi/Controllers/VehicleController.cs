using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vehicle.Aplication.Interfaces;
using VehicleApi.DTOs;
using Entities = Vehicle.Domain.Entities;

namespace VehicleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IMapper _mapper;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(IVehicleService vehicleService, IMapper mapper, ILogger<VehicleController> logger)
        {
            //GUARDS ADD
            _vehicleService = vehicleService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> Get()
        {
            _logger.LogInformation($"RetrieveVehicleAsync");

            var result = await _vehicleService.Get();
            var vehicles = _mapper.Map<IEnumerable<VehicleDTO>>(result);
            return Ok(vehicles);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<VehicleDTO>> Get(int id)
        {
            _logger.LogInformation($"RetrieveVehicleById - {id}");

            var result = await _vehicleService.Get(id);
            var vehicle = _mapper.Map<VehicleDTO>(result);
            return Ok(vehicle);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VehicleDTO request)
        {
            _logger.LogInformation($"InsertVehicleAsync - {JsonSerializer.Serialize(request)}");

            var vehicle = _mapper.Map <Entities.Vehicle>(request);
            await _vehicleService.Create(vehicle);
            return Ok(request);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] VehicleDTO request)
        {
            _logger.LogInformation($"UpdateVehicleAsync - {JsonSerializer.Serialize(request)}");

            var vehicle = _mapper.Map<Entities.Vehicle>(request);
            await _vehicleService.Update(vehicle);
            return Ok(vehicle.Id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"DeleteVehicleAsync - {id}");

            await _vehicleService.Delete(id);            
            return Ok(id);
        }
    }
}
