using MedSave.DTOs;
using MedSave.DTOs.Manufacturer;
using MedSave.Repositories;
using MedSave.Services.Manufacturer;
using Microsoft.AspNetCore.Mvc;

namespace MedSave.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManufacturerController : ControllerBase
{
    private readonly IManufacturerService _manufacturerService;

    public ManufacturerController(IManufacturerService manufacturerService)
    {
        _manufacturerService = manufacturerService;
    }
    
    /// <summary>
    /// Retorna todos os Fornecedores cadastrados.
    /// </summary>
    /// <remarks>
    /// Endpoint não paginado. Retorna a coleção completa de fornecedores, sem detalhes de contato e endereço.
    ///
    /// Status possíveis:
    /// - 200 OK: coleção retornada com sucesso
    /// - 404 Not Found: nenhum fornecedor encontrado
    /// - 500 Internal Server Error: erro inesperado ao buscar os dados
    /// </remarks>>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var dtos = await _manufacturerService.GetAllAsync();

            return Ok(new { items = dtos });
        }
        catch (ManufacturerRepository.NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when searching for the manufacturers",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtém um Fornecedor específico pelo ID.
    /// </summary>
    /// <param name="id">ID do Fornecedor.</param>
    /// <remarks>
    /// Endpoint que retorna o fornecedor em especifico com detalhes sobre o contato e endereço do mesmo.
    ///
    /// Status possíveis:
    ///
    /// - 200 OK: Fornecedor encontrado e retornado no corpo da resposta
    /// - 404 Not Found: nenhum fornecedor com o ID informado foi encontrado
    /// - 500 Internal Server Error: erro inesperado no servidor
    /// </remarks>
    /// <returns>Recurso <see cref="ManufacturerDTO"/></returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var dto = await _manufacturerService.GetByIdAsync(id);

            return Ok(dto);
        }

        catch (ManufacturerRepository.NotFoundException ex)
        {
            return StatusCode(404, ex.Message);
        }
        
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when searching for the manufacturer",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Cadastra um novo Fornecedor no banco de dados.
    /// </summary>
    /// <param name="dto">Dados do Fornecedor a ser criado.</param>
    /// <remarks>
    /// Endpoint que cria um fornecedor, sendo necessário dados de contato e endereço do mesmo.
    ///
    /// Campos Gerados Automaticamente:
    /// - <c>manufacId</c>: gerado pelo banco de dados (IDENTITY)
    /// - <c>contactManuId</c>: gerado pelo banco de dados (IDENTITY)
    /// - <c>addressIdManufacturer</c>: gerado pelo banco de dados (IDENTITY)
    ///
    /// Corpo esperado (JSON) - apenas os campos que o cliente precisa enviar:
    /// <code>
    ///    {
    ///        "manufacturerDto": {
    ///            "nameManu": "string",
    ///            "cnpj": 12345678912345
    ///        },
    ///        "contactManufacturerDto": {
    ///            "emailManu": "string",
    ///            "phoneNumberManu": 11912345678
    ///        },
    ///        "addressManufacturerDto": {
    ///            "complement": "N/A",
    ///            "numberManu": 123,
    ///            "addressDescription": "Rua da Silva",
    ///            "cep": 12345678,
    ///            "neighId": 1
    ///        }
    ///    }
    /// </code>
    ///
    /// Status possíveis:
    /// - 201 Created: fornecedor cadastrado com sucesso
    /// - 400 Bad Request: payload inválido ou inconsistente
    /// - 409 Conflict: Informação única já cadastrada
    /// - 500 Internal Server Error: erro inesperado ao cadastrar o fornecedor
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddManufacturer([FromBody] CreateManufacturerRequest req)
    {
        try
        {
            var created = await _manufacturerService.AddAsync(req.ManufacturerDto, req.AddressManufacturerDto, req.ContactManufacturerDto);
            
            return CreatedAtAction(nameof(GetById), new {id = created.ManufacId}, created);
        }

        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = "Invalid request body." });
        }

        catch (ManufacturerService.ConflictException ex)
        {
            return StatusCode(409, new { message = "CNPJ already registered." });
        }
        
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when adding the manufacturer",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Atualiza as informações de Fornecedor existente.
    /// </summary>
    /// <param name="id">ID do Fornecedor.</param>
    /// <remarks>
    /// Endpoint que atualiza as informações de cadastro de um fornecedor cadastrado no banco de dados.
    /// Não atualiza as informações de contato e endereço.
    ///
    /// O ID do fornecedor só é necessário no campo especificado para o mesmo.
    /// Caso a mesma informação seja encontrada no corpo de requisição, o mesmo presente no corpo de requisição será ignorado.
    ///
    /// <code>
    /// {
    ///     "nameManu": "string",
    ///     "cnpj": 0,
    ///     "contactManuId": 0,
    ///     "addressIdManufacturer": 0
    /// }
    /// </code>
    ///
    /// Status possíveis:
    /// - 200 OK: fornecedor cadastrado com sucesso
    /// - 404 Not Found: nenhum fornecedor com o ID informado foi encontrado
    /// - 400 Bad Request: payload inválido ou inconsistente
    /// - 500 Internal Server Error: erro inesperado ao cadastrar o fornecedor
    /// </remarks>
    /// <param name="ID"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateManufacturer(long id, [FromBody] ManufacturerDTO dto)
    {
        try
        {
            dto.ManufacId = id;

            await _manufacturerService.UpdateAsync(id, dto);
            return StatusCode(200, new { message = $"Manufacturer with id {id} updated" });
        }

        catch (ManufacturerService.NotFoundException ex)
        {
            return NotFound(new { message = $"Manufacturer with Id {id} not found" });
        }
        
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when adding the manufacturer",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Deleta um Fornecedor pelo ID.
    /// </summary>
    /// <param name="id">ID do Fornecedor.</param>
    /// <remarks>
    /// Endpoint para deletar o cadastro de um fornecedor do banco de dados.
    ///
    /// Status possíveis:
    /// - 200 OK: fornecedor deletado com sucesso
    /// - 404 Not Found: nenhum fornecedor com o ID informado foi encontrado
    /// - 500 Internal Server Error: erro inesperado ao cadastrar o fornecedor
    /// </remarks>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteManufacturer(long id)
    {
        try
        {
            await _manufacturerService.DeleteAsync(id);

            return Ok(new { message = $"Manufacturer with Id {id} deleted" });
        }
        
        catch (ManufacturerService.NotFoundException ex)
        {
            return NotFound(new { message = $"Manufacturer with Id {id} not found" });
        }
        
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when deleting the manufacturer",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Busca fabricantes com filtros opcionais, paginação e ordenação.
    /// </summary>
    /// <param name="cnpj">CNPJ do fabricante para filtro opcional.</param>
    /// <param name="contactManuId">ID do contato do fabricante para filtro opcional.</param>
    /// <param name="addressIdManufacturer">ID do endereço do fabricante para filtro opcional.</param>
    /// <param name="page">Número da página (padrão 1, mínimo 1).</param>
    /// <param name="pageSize">Tamanho da página (padrão 10, máximo conforme regra da aplicação).</param>
    /// <param name="sortBy">Campo de ordenação. Padrão: <c>manufacId</c>.</param>
    /// <param name="sortDir">Direção da ordenação: <c>asc</c> ou <c>desc</c>.</param>
    /// <remarks>
    /// Retorna os fornecedores correspondentes a pesquisa sem detalhes de contato e endereço
    /// 
    /// Exemplo de chamada:
    /// <code>
    /// GET /api/Manufacturer/search?cnpj=123456789&amp;page=1&amp;pageSize=10&amp;sortBy=manufacId&amp;sortDir=asc
    /// </code>
    ///
    /// Status possíveis:
    /// - 200 OK: resultados retornados com sucesso (pode vir lista vazia)
    /// - 400 Bad Request: parâmetros inválidos para filtro, paginação ou ordenação
    /// - 500 Internal Server Error: erro inesperado ao realizar a busca
    /// </remarks>
    /// <returns>Coleção paginada de fabricantes com informações de paginação.</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Search(
        [FromQuery] int? cnpj,
        [FromQuery] long? contactManuId,
        [FromQuery] long? addressIdManufacturer,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "manufacId",
        [FromQuery] string sortDir = "asc"
    )
    {
        try
        {
            var result = await _manufacturerService.SearchAsync(cnpj, contactManuId, addressIdManufacturer, page, pageSize, sortBy, sortDir);

            return Ok(new { Items = result.Items, PageInfo = result.PageInfo });
        }

        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when searching the manufacturer",
                details = ex.Message
            });
        }
    }
}