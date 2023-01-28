using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using Rifa.Models;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;
using System.Text;

namespace Rifa.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost()]
        public IActionResult Enviar([FromForm] Rifas rifa)
        {
            return Print(rifa);
        }
        [HttpGet()]
        public IActionResult Enviar()
        {

            return View();
        }

        private FileContentResult Print(Rifas rifa)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\rifa.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();


            rifa.Titulo = string.IsNullOrWhiteSpace(rifa.Titulo) ? "Rifa festa de aniversário" : rifa.Titulo;
            rifa.Rodape = string.IsNullOrWhiteSpace(rifa.Rodape) ? "O sorteio será pelos 4 últimos números do 1º " +
                "prêmio da loteria " : rifa.Rodape;
            rifa.Valor = rifa.Valor == 0 ? 10 : rifa.Valor;
            rifa.Premio = string.IsNullOrWhiteSpace(rifa.Premio) ? "Kit churasco" : rifa.Premio;

            var imageParam = "";
            var pathImg = $"{_webHostEnvironment.WebRootPath}\\img\\churrasco.png";

            using (var b = new Bitmap(pathImg))
            {
                using (var ms = new MemoryStream())
                {
                    b.Save(ms, ImageFormat.Bmp);
                    imageParam = Convert.ToBase64String(ms.ToArray());
                }
            }

            parameters.Add(nameof(rifa.Titulo), rifa.Titulo);
            parameters.Add(nameof(rifa.Rodape), rifa.Rodape);
            parameters.Add(nameof(rifa.Valor), rifa.Valor.ToString("F2"));
            parameters.Add(nameof(rifa.Premio), rifa.Premio);
            parameters.Add(nameof(rifa.Img), imageParam);

            LocalReport localRedirect = new LocalReport(path);

            List<RifaCod> rifaCods = new List<RifaCod>();

            for (int i = 1; i < 201; i++)
            {
                rifaCods.Add(new RifaCod()
                {
                    Codigo = i.ToString().PadLeft(4,'0')
                });
            }

            localRedirect.AddDataSource("DataSet1", rifaCods);

            var result = localRedirect.Execute(RenderType.Pdf, extension, parameters, mimtype);
            return File(result.MainStream, MediaTypeNames.Application.Octet, "rifas.pdf");


        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        class RifaCod
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
        }
    }
}