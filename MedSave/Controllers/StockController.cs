using MedSave.DTOs.Hypermedia;
using MedSave.DTOs.Stock;
using MedSave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSave.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly LinkGenerator _links;

        public StockController(IStockService stockService, LinkGenerator links)
        {
            _stockService = stockService;
            _links = links;
        }
        
        
        private string Href(string routeName, object? values = null)
            => _links.GetPathByAction(HttpContext, action: routeName, controller: "Stock", values: values) ?? "#";

        private IEnumerable<Link> StockItemLinks(long id) => new[]
        {
            new Link { Rel = "self",   Href = Href(nameof(GetById), new { id }), Method = "GET" },
            new Link { Rel = "update", Href = Href(nameof(Update),  new { id }), Method = "PUT" },
            new Link { Rel = "list",   Href = Href(nameof(GetAll)),              Method = "GET" },
            new Link { Rel = "search", Href = Href(nameof(Search)),              Method = "GET" }
        };

        private IEnumerable<Link> StockCollectionLinks(int page, int pageSize, int totalPages, object? filters = null)
        {
            var baseValues = new Dictionary<string, object?>
            {
                ["page"] = page,
                ["pageSize"] = pageSize
            };

            if (filters is not null)
            {
                foreach (var prop in filters.GetType().GetProperties())
                    baseValues[prop.Name] = prop.GetValue(filters);
            }

            var links = new List<Link>
            {
                new Link { Rel = "self", Href = Href(nameof(Search), baseValues), Method = "GET" }
            };

            if (page > 1)
            {
                var prev = new Dictionary<string, object?>(baseValues) { ["page"] = page - 1 };
                links.Add(new Link { Rel = "prev", Href = Href(nameof(Search), prev), Method = "GET" });
            }

            if (page < totalPages)
            {
                var next = new Dictionary<string, object?>(baseValues) { ["page"] = page + 1 };
                links.Add(new Link { Rel = "next", Href = Href(nameof(Search), next), Method = "GET" });
            }
            
            links.Add(new Link { Rel = "all", Href = Href(nameof(GetAll)), Method = "GET" });

            return links;
        }
        
        /// <summary>
        /// Returns all registered Stock.
        /// </summary>
        /// <remarks>
        /// Non-paginated endpoint. Returns the complete collection of Stock, with HATEOAS.
        ///
        /// Possible status codes:
        /// - 200 OK: collection returned successfully
        /// - 500 Internal Server Error: unexpected error while retrieving data
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var dtos = await _stockService.GetAllAsync();

                var items = dtos.Select(d => new Resource<StockDTO>
                {
                    Data = d,
                    _links = StockItemLinks(d.StockId)
                });

                var collection = new CollectionResource<StockDTO>
                {
                    Items = items,
                    PageInfo = new { page = 1, pageSize = dtos.Count(), totalItems = dtos.Count(), totalPages = 1 },
                    _links = new[]
                    {
                        new Link { Rel = "self", Href = Href(nameof(GetAll)), Method = "GET" },
                        new Link { Rel = "search", Href = Href(nameof(Search)), Method = "GET" }
                    }
                };

                return Ok(collection);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error when searching for the stock", details = ex.Message });
            }
        }


        /// <summary>
        /// Gets a specific Stock by ID.
        /// </summary>
        /// <param name="id">Stock ID.</param>
        /// <remarks>
        /// Endpoint that returns a specific Stock with HATEOAS.
        ///
        /// Possible status codes:
        ///
        /// - 200 OK: Stock found and returned in the response body
        /// - 404 Not Found: no Stock with the provided ID was found
        /// - 500 Internal Server Error: unexpected server error
        /// </remarks>
        /// <returns>Resource <see cref="StockDTO"/></returns>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var dto = await _stockService.GetByIdAsync(id);

                var resource = new Resource<StockDTO>
                {
                    Data = dto!,
                    _links = StockItemLinks(id)
                };

                return Ok(resource);
            }
            catch (Exception ex) when (ex.Message.Contains("not founded", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error when searching for the stock", details = ex.Message });
            }
        }


        /// <summary>
        /// Updates the information of an existing Stock.
        /// </summary>
        /// <param name="id">Stock ID.</param>
        /// <remarks>
        /// Endpoint that updates the registration information of a Stock stored in the database.
        ///
        /// The Stock ID is only required in the specified route parameter.
        /// If the same information is found in the request body, the value present in the request body will be ignored.
        ///
        /// <code>
        /// {
        ///     "quantity": 0,
        ///     "batchId": 0,
        ///     "medicineId": 0,
        ///     "healthcareProviderId": 0
        /// }
        /// </code>
        ///
        /// Possible status codes:
        /// - 200 OK: Stock updated successfully
        /// - 404 Not Found: no Stock with the provided ID was found
        /// - 400 Bad Request: invalid or inconsistent payload
        /// - 500 Internal Server Error: unexpected error while updating the Stock
        /// </remarks>
        /// <param name="ID"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] StockDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid Payload." });

            if (dto.StockId != 0 && dto.StockId != id)
                return BadRequest(new { message = "The path ID differs from the body ID." });

            dto.StockId = id;

            try
            {
                await _stockService.UpdateAsync(dto);
                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not founded", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { message = ex.Message });

                return StatusCode(500, new { message = "Error when updating stock.", details = ex.Message });
            }
        }
        
        /// <summary>
        /// Searches Stock with optional filters, pagination, and sorting.
        /// </summary>
        /// <param name="medicineId">Medicine ID for optional filtering.</param>
        /// <param name="healthcareProviderId">Healthcare provider ID for optional filtering.</param>
        /// <param name="batchId">Batch ID for optional filtering.</param>
        /// <param name="page">Page number. Default: 1, minimum: 1.</param>
        /// <param name="pageSize">Page size. Default: 10, maximum according to the application rule.</param>
        /// <param name="sortBy">Sorting field. Default: <c>healthcareProviderId</c>.</param>
        /// <param name="sortDir">Sorting direction: <c>asc</c> or <c>desc</c>.</param>
        /// <remarks>
        /// Returns the Stock matching the search criteria, with HATEOAS.
        ///
        /// Possible status codes:
        /// - 200 OK: results returned successfully. The list may be empty
        /// - 400 Bad Request: invalid filter, pagination, or sorting parameters
        /// - 500 Internal Server Error: unexpected error while performing the search
        /// </remarks>
        /// <returns>Paginated collection of manufacturers with pagination information.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] long? medicineId,
            [FromQuery] long? healthcareProviderId,
            [FromQuery] long? batchId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "stockId",
            [FromQuery] string sortDir = "asc")
        {
            try
            {
                var result = await _stockService.SearchAsync(
                    medicineId, healthcareProviderId, batchId,
                    page, pageSize, sortBy, sortDir
                );

                var items = result.Items.Select(d => new Resource<StockDTO>
                {
                    Data = d,
                    _links = StockItemLinks(d.StockId)
                });

                var collection = new CollectionResource<StockDTO>
                {
                    Items = items,
                    PageInfo = result.PageInfo,
                    _links = StockCollectionLinks(
                        page: result.PageInfo.Page,
                        pageSize: result.PageInfo.PageSize,
                        totalPages: result.PageInfo.TotalPages,
                        filters: new { medicineId, healthcareProviderId, batchId, sortBy, sortDir }
                    )
                };

                return Ok(collection);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error when searching stock.", details = ex.Message });
            }
        }
    }
}
