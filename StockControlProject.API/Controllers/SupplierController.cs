using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControlProject.Domain.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IGenericService<Supplier> _service;

        //localhost:PortNo/api/Category/TumKategorileriGetir

        public SupplierController(IGenericService<Supplier> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult TumTedarikcileriGetir()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet]
        public IActionResult AktifTedarikcileriGetir()
        {
            return Ok(_service.GetActive());
        }

        [HttpGet("{id}")]
        public IActionResult IdyeGoreTedarikciGetir(int id)
        {
            return Ok(_service.GetByID(id));
        }

        [HttpPost]
        public IActionResult TedarikciEkle(Supplier supplier)
        {
            _service.Add(supplier);
            //return Ok("Başarılı");
            return CreatedAtAction("IdyeGoreTedarikciGetir", new { id = supplier.ID }, supplier);
        }
        [HttpPut("{id}")]
        public IActionResult TedarikciGuncelle(int id, Supplier supplier)
        {
            if (id != supplier.ID)
            {
                return BadRequest();
            }
            if (!TedarikciVarMi(id))
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _service.Update(supplier);
                    return Ok(supplier);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            

          
        }

        private bool TedarikciVarMi(int id)
        {
            return _service.Any(sup => sup.ID == id);
        }

        [HttpDelete("{id}")]
        public IActionResult TedarikciSil(int id)
        {
            var supplier = _service.GetByID(id);
            if (supplier == null)
            {
                return NotFound();
            }

            try
            {
                _service.Remove(supplier);
                return Ok("Tedarikçi silindi");
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult TedarikciAktifleştir(int id)
        {
            var supplier = _service.GetByID(id);
            if (supplier == null)
            {
                return NotFound();
            }

            try
            {
                _service.Activate(id);
                return Ok(_service.GetByID(id));
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
    }
}
