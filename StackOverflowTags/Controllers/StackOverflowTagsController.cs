using Application.Dtos;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace StackOverflowTags.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StackOverflowTagsController : ControllerBase
    {
        private readonly ITagProviderService _tagProviderService;
        private readonly ITagLoaderService _tagLoaderService;

        public StackOverflowTagsController(ITagProviderService tagProviderService, ITagLoaderService tagLoaderService)
        {
            _tagProviderService = tagProviderService ?? throw new ArgumentNullException(nameof(tagProviderService));
            _tagLoaderService = tagLoaderService ?? throw new ArgumentNullException(nameof(tagLoaderService));
        }

        [HttpGet]
        [SwaggerOperation(
            Description = "This endpoint retrieves tags filtered by page, pageSize (0 - 100), sortBy (Name, Percentage), sortOrder (Asc, Desc)"
        )]
        [ProducesResponseType(typeof(PaginatedTagsResult), 200)]
        [SwaggerResponse(500, "An unexpected error occurred.")]
        [SwaggerResponse(400, "Invalid OrderBy value: {value}")]
        [SwaggerResponse(400, "Invalid SortBy value: {value}")]
        [SwaggerResponse(400, "Page must be greater than 0. Passed value: {value}")]
        [SwaggerResponse(400, "PageSize must be between 1 and 100. Passed value: {value}")]
        public async Task<IActionResult> GetTags(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "name",
            [FromQuery] string sortOrder = "asc") 
        {
            var filter = new TagFilter(page, pageSize, sortBy, sortOrder);

            var result = await _tagProviderService.GetTagsAsync(filter).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(
            Description = "This endpoint remove old tags and load new 1000 tags to database)"
        )]
        [SwaggerResponse(500, "An unexpected error occurred.")]
        [SwaggerResponse(200)]
        public IActionResult LoadTags()
        {
            const int targetFetchCount = 1000;
            const int page = 100;
            _tagLoaderService.FetchAndStoreTagsAsync(targetFetchCount, page).ConfigureAwait(false);

            return Ok();
        }
    }
}
