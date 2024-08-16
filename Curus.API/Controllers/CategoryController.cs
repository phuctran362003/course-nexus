using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Enum;

namespace Curus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminPolicy")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Gets a list of all categories with optional paging and sorting.
        /// </summary>
        /// <param name="pageNumber">The page number (default is 1).</param>
        /// <param name="pageSize">The page size (default is 10).</param>
        /// <param name="sortBy">The field to sort by (default is Id).</param>
        /// <param name="sortDirection">The sort direction (asc or desc, default is asc).</param>
        /// <returns>A paged and sorted list of categories.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] CategorySortOptions sortBy = CategorySortOptions.Id, [FromQuery] SortDirection sortDirection = SortDirection.Asc)
        {
            try
            {
                var categories = await _categoryService.GetCategories(pageNumber, pageSize, sortBy, sortDirection);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        
        /// <summary>
        /// Gets a specific category by ID.
        /// </summary>
        /// <returns>The category.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryDto">The category data transfer object.</param>
        /// <returns>The created category.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryDTO categoryDto)
        {
            try
            {
                var category = await _categoryService.CreateCategory(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        
        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <param name="categoryDto">The category data transfer object.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            try
            {
                await _categoryService.UpdateCategory(id, categoryDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        /// <summary>
        /// Deletes an existing category (soft delete).
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategory(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        /// <summary>
        /// Deactivates an existing category.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <returns>No content.</returns>
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            try
            {
                await _categoryService.DeactivateCategory(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Activates a deactivated category.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <returns>No content.</returns>
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateCategory(int id)
        {
            try
            {
                await _categoryService.ActivateCategory(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        
    }
}