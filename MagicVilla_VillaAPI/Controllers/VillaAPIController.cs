using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class VillaAPIController : ControllerBase
  {
    private readonly ILogger<VillaAPIController> logger;
    public VillaAPIController(ILogger<VillaAPIController> _logger)
    {
      logger = _logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<VillaDTO>> GetVillas()
    {
      logger.LogInformation("Logging GetVillas");
      return Ok(VillaStore.villaList);
    }

    [HttpGet("id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> GetVilla(int id)
    {
      if (id == 0)
        return BadRequest();

      VillaDTO villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
      if (villa == null) 
        return NotFound();

      return Ok(villa);      
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (villaDTO == null || villaDTO.Id > 0)
        return BadRequest();

      villaDTO.Id = VillaStore.villaList.Select(c => c.Id).Max() + 1;
      VillaStore.villaList.Add(villaDTO);

      return CreatedAtRoute("GetVilla", new {id = villaDTO.Id}, villaDTO);
    }

    [HttpDelete("id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> DeleteVilla(int id)
    {
      if (id == 0)
        return BadRequest();

      var villa = VillaStore.villaList.FirstOrDefault(c => c.Id == id);
      if (villa == null)
        return NotFound();

      VillaStore.villaList.Remove(villa);

      return NoContent();
    }

    [HttpPut("id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
    {
      if (!ModelState.IsValid)
        return BadRequest();

      if (villaDTO == null || id != villaDTO.Id)
        return BadRequest();

      var villa = VillaStore.villaList.FirstOrDefault(c => c.Id == id);
      if (villa == null)
        return NotFound();

      villa.Name = villaDTO.Name;
      villa.Occupancy = villaDTO.Occupancy;
      villa.Sqft = villaDTO.Sqft;

      return NoContent();
    }

    [HttpPatch("id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaDTO> patchDTO)
    {
      if (id == 0)
        return BadRequest();

      var villa = VillaStore.villaList.FirstOrDefault(c => c.Id == id);
      if (villa == null)
        return NotFound();

      patchDTO.ApplyTo(villa, ModelState);
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return NoContent();
    }
  }
}
