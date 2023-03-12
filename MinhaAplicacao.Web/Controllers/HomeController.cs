using Microsoft.AspNetCore.Mvc;
using MinhaAplicacao.Negocio;
using MinhaAplicacao.Web.Models;
using System.Diagnostics;

namespace MinhaAplicacao.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Controlador Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }
        /// <summary>
        /// Controlador Importação
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Importacao()
        {
            await Task.CompletedTask;
            return View();
        }

        /// <summary>
        /// Controlador Importação (Post) onde será realizada as chamadas para efeturar 
        /// os cálculos e exportação do Json
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Importacao(IFormFile[] files)
        {
            try
            {
                if (files != null && files.Count() > 0)
                {
                   var objFInal = await new ImportacaoNegocio().ProcessarImportacao(files);
                    //Transformando o objeto final para Json
                    var jsonstr = System.Text.Json.JsonSerializer.Serialize(objFInal);
                    byte[] byteArray = System.Text.ASCIIEncoding.Latin1.GetBytes(jsonstr);

                    return File(byteArray, "application/force-download", DateTime.Now.ToString("dd_MM-yyy_HHmmssfff") + ".json");
                }
            }
            catch (Exception ex)
            {
              ViewBag.Error = "Erro ao processar arquivos. Detalhes do Erro: " + ex.Message;   
            }
            
            await Task.CompletedTask;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}