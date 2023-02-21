using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockControlProject.Domain.Entities;
using System.Text;

namespace StockControlProject.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        string uri = "https://localhost:7291";
        public async Task<IActionResult> Index()
        {
            List<Category> kategoriler = new List<Category>();
            using (var httpClient = new HttpClient())
            {
                using (var cevap = await httpClient.GetAsync($"{uri}/api/Category/TumKategorileriGetir"))
                {
                    string apiCevap = await cevap.Content.ReadAsStringAsync();
                    kategoriler = JsonConvert.DeserializeObject<List<Category>>(apiCevap);
                }
            }
            return View(kategoriler);
        }
        public async Task<IActionResult> ActivateCategory(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var cevap = await httpClient.GetAsync($"{uri}/api/Category/KategoriAktivate/{id}"))
                {

                }
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteCategory(int id)
        {


            using (var httpClient = new HttpClient())
            {
                using (var cevap = await httpClient.DeleteAsync($"{uri}/api/Category/KategoriSil/{id}"))
                {

                }
            }
            return RedirectToAction("Index");
        }

        static Category updatedCategory;
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            
            using (var httpClient = new HttpClient())
            {
                using (var cevap = await httpClient.GetAsync($"{uri}/api/Category/TumKategorileriGetir/{id}"))
                {
                    string apiCevap = await cevap.Content.ReadAsStringAsync();
                    updatedCategory = JsonConvert.DeserializeObject<Category>(apiCevap);
                    return View(updatedCategory);
                }
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            using (var httpClient = new HttpClient())
            {

                using (var cevap = await httpClient.PutAsJsonAsync($"{uri}/api/Category/KategoriGuncelle/{category.ID}", category))
                {
                    if (cevap.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                        return View(cevap);
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            category.IsActive = true; 
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                using (var cevap = await httpClient.PostAsJsonAsync($"{uri}/api/Category/KategoriEkle", category))
                {
                    if (cevap.IsSuccessStatusCode)
                    {
                        string apiCevap = await cevap.Content.ReadAsStringAsync();
                    }
                    else
                        return View(cevap);
                }
            }
            return RedirectToAction("Index");
        }

    }
}
