using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControlProject.Domain.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IGenericService<Product> _service;

        //localhost:PortNo/api/Category/TumKategorileriGetir

        public ProductController(IGenericService<Product> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult TumUrunleriGetir()
        {
            return Ok(_service.GetAll(t0=>t0.Category, t0=>t0.Supplier));
        }

        [HttpGet]
        public IActionResult AktifUrunleriGetir()
        {
            return Ok(_service.GetActive(t0 => t0.Category, t0 => t0.Supplier));
        }

        [HttpGet("{id}")]
        public IActionResult IdyeGoreUrunGetir(int id)
        {
            return Ok(_service.GetByID(id));
        }

        [HttpPost]
        public IActionResult UrunEkle(Product product)
        {
            _service.Add(product);
            //return Ok("Başarılı");
            return CreatedAtAction("IdyeGoreUrunGetir", new { id = product.ID }, product);
        }
        [HttpPut("{id}")]
        public IActionResult TedarikciGuncelle(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }
            if (!UrunVarMi(id))
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _service.Update(product);
                    return Ok(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }



        }

        private bool UrunVarMi(int id)
        {
            return _service.Any(product => product.ID == id);
        }

        [HttpDelete("{id}")]
        public IActionResult UrunSil(int id)
        {
            var product = _service.GetByID(id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _service.Remove(product);
                return Ok("Ürün silindi");
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult UrunAktifleştir(int id)
        {
            var product = _service.GetByID(id);
            if (product == null)
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
