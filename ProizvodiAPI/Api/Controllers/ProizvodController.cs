using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProizvodiAPI.Application.DTOs;
using ProizvodiAPI.Application.Proizvodi.Command;
using ProizvodiAPI.Application.Proizvodi.Queries;

namespace ProizvodiAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProizvodController : ControllerBase
    {
        private readonly IValidator<KreirajProizvodDTO> _validator;
        private readonly IMediator _mediator;

        public ProizvodController(
            IValidator<KreirajProizvodDTO> validator,
            IMediator mediator
        )
        {
            _validator = validator;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProizvodResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DajSveProizovde()
        {
            var proizvodi = await _mediator.Send(new DajSveProizvodeQuery());

            return Ok(proizvodi);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProizvodResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DajProzivodPoId(int id)
        {
            var proizvod = await _mediator.Send(new DajProizvodPoIdQuery(id));

            if (proizvod == null)
                return NotFound($"Proizvod sa ID {id} nije pronađen!");

            return Ok(proizvod);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProizvodResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> KreirajProizvod([FromBody] KreirajProizvodDTO dto)
        {
            // Validacija
            var validacijaRezultat = await _validator.ValidateAsync(dto);
            if (!validacijaRezultat.IsValid)
            {
                var greske = validacijaRezultat.Errors
                    .Select(e => new { Polje = e.PropertyName, Poruka = e.ErrorMessage });
                return BadRequest(greske);
            }

            var proizvod = await _mediator.Send(new KreirajProizovdCommand(dto));
            return CreatedAtAction(nameof(DajProzivodPoId), new { id = proizvod.Id }, proizvod);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProizvodResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AzurirajProizvod(int id, [FromBody] AzurirajProizvodDTO dto)
        {
            var proizvod = await _mediator.Send(new IzmeniProizvodCommand(id, dto));

            if (proizvod == null)
                return NotFound($"Proizvod sa ID {id} nije pronađen!");

            return Ok(proizvod);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ObrisiProizvod(int id)
        {
            var rezultat = await _mediator.Send(new ObrisiProizvodCommand(id));
            if (!rezultat)
                return NotFound($"Proizvod sa ID {id} nije pronađen!");
            return NoContent();
        }
    }
}
