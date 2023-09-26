using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vehicle.Aplication.Interfaces;
using Vehicle.Domain.Entities;
using VehicleApi.DTOs;

namespace VehicleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService vehicleService, IMapper mapper, ILogger<CategoryController> logger)
        {
            //GUARDS ADD
            _categoryService = vehicleService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
        {
            _logger.LogInformation($"RetrieveCategoryAsync");

            var result = await _categoryService.Get();
            var vehicles = _mapper.Map<IEnumerable<CategoryDTO>>(result);
            return Ok(vehicles);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CategoryDTO>> Get(int id)
        {
            _logger.LogInformation($"RetrieveCategoryById - {id}");

            var result = await _categoryService.Get(id);
            var vehicle = _mapper.Map<CategoryDTO>(result);
            return Ok(vehicle);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryDTO request)
        {
            _logger.LogInformation($"InsertCategoryAsync - {JsonSerializer.Serialize(request)}");

            var vehicle = _mapper.Map<Category>(request);
            await _categoryService.Create(vehicle);
            return Ok(request);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CategoryDTO request)
        {
            _logger.LogInformation($"UpdateCategoryAsync - {JsonSerializer.Serialize(request)}");

            var vehicle = _mapper.Map<Category>(request);
            await _categoryService.Update(vehicle);
            return Ok(vehicle.Id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"DeleteVehicleAsync - {id}");

            await _categoryService.Delete(id);
            return Ok(id);
        }
    }
}
