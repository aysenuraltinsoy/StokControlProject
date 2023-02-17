using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControlProject.Domain.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGenericService<User> _service;

        //localhost:PortNo/api/Category/TumKategorileriGetir

        public UserController(IGenericService<User> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult TumKullanıcılarıGetir()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet]
        public IActionResult AktifKullanıcılarıGetir()
        {
            return Ok(_service.GetActive());
        }

        [HttpGet("{id}")]
        public IActionResult IdyeGoreKullanıcıGetir(int id)
        {
            return Ok(_service.GetByID(id));
        }

        [HttpPost]
        public IActionResult KullanıcıEkle(User user)
        {
            _service.Add(user);
            //return Ok("Başarılı");
            return CreatedAtAction("IdyeGoreKullanıcıGetir", new { id = user.ID }, user);
        }
        [HttpPut("{id}")]
        public IActionResult KullanıcıGuncelle(int id, User user)
        {
            if (id != user.ID)
            {
                return BadRequest();
            }
            if (!KullaniciVarMi(id))
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _service.Update(user);
                    return Ok(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }



        }

        private bool KullaniciVarMi(int id)
        {
            return _service.Any(user => user.ID == id);
        }

        [HttpDelete("{id}")]
        public IActionResult KullaniciSil(int id)
        {
            var user = _service.GetByID(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                _service.Remove(user);
                return Ok("Kullanıcı silindi");
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult KullanıcıAktifleştir(int id)
        {
            var user = _service.GetByID(id);
            if (user == null)
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

        [HttpGet]
        public IActionResult Login(string email, string parola)
        {
            if (_service.Any(user=>user.Email==email && user.Password==parola))
            {
                User loggedUser = _service.GetByDefault(user => user.Email == email && user.Password == parola);
                return Ok(loggedUser);
            }
            return NotFound();
        }
    }
}
