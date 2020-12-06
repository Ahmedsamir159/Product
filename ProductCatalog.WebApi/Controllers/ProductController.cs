using System;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Web.Http.Cors;

using ProductCatalog.Data;
using ProductCatalog.Data.Models.Entities;
using System.IO;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using ClosedXML.Excel;
using System.Web.Mvc;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace ProductCatalog.WebApi.Controllers

{
    [RoutePrefix("api/Product")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ProductController : ApiController
    {
        private readonly IRepository<Product> _productRepository;

        public ProductController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        // GET: api/Product
        [HttpGet]
        public IList<Product> GetProducts(string key = null)
        {
            if (string.IsNullOrEmpty(key))
                return _productRepository.ToList();
            else
                return GetProductsByQuery(key);
        }

        [HttpGet]
        [Route("GetProductsByQuery")]
        public IList<Product> GetProductsByQuery(string query)
        {
            return _productRepository.Get(p => p.Name.ToLower().Contains(query.ToLower()));
        }

        // GET: api/Product/5
        [HttpGet]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            var product = _productRepository.First(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet]
        [Route("export")]
        public HttpResponseMessage Get00()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            var products = GetProducts();


            writer.WriteLine("Name,Photo,Price,Last Updated");

            foreach (var product in products)
            {

                writer.WriteLine(product.Name + "," + product.Photo + "," + product.Price + "," + product.LastUpdated.ToShortDateString());
            }
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Export.csv" };
            return result;
        }
        //public IHttpActionResult Export000(string q)
        //{
        //    var products = GetProducts();

        //    var table = new System.Data.DataTable("productCatalog");
        //    table.Columns.Add("Id", typeof(int));
        //    table.Columns.Add("Name", typeof(string));
        //    table.Columns.Add("Photo", typeof(string));
        //    table.Columns.Add("Price", typeof(string));
        //    table.Columns.Add("Last Updated", typeof(string));

        //    foreach (var product in products)
        //    {
        //        table.Rows.Add(product.Id, product.Name, product.Photo, product.Price, product.LastUpdated.ToShortDateString());
        //    }

        //    var grid = new GridView { DataSource = table };
        //    grid.DataBind();

        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=ProductCatalog.xls");
        //    Response.ContentType = "application/ms-excel";

        //    Response.Charset = "";
        //    StringWriter sw = new StringWriter();
        //    HtmlTextWriter htw = new HtmlTextWriter(sw);

        //    grid.RenderControl(htw);

        //    Response.Output.Write(sw.ToString());
        //    Response.Flush();
        //    Response.End();
        //    return BadRequest();

        //}
        [System.Web.Http.HttpPut]
        // PUT: api/Product/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult PutProduct(int id)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            var HttpBody = HttpContext.Current.Request.Form["product"];
            HttpBody = HttpBody.Replace("null", "0");
            Product product = JsonConvert.DeserializeObject<Product>(HttpBody);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];

                    imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    imageName = imageName + DateTime.UtcNow.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filepath = HttpContext.Current.Server.MapPath("~/Image/" + imageName);
                    postedFile.SaveAs(filepath);

                    product.Photo = imageName;
                    product.LastUpdated = DateTime.UtcNow;
                    _productRepository.Update(product);
                    _productRepository.Commit();
                }
            }
            product.LastUpdated = DateTime.UtcNow;

            _productRepository.Update(product);

            try
            {
                _productRepository.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return StatusCode(HttpStatusCode.OK);
        }
       
        [HttpGet]
        [Route("download")]
        public string DownloadExcelDocument(string query=null)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = Guid.NewGuid().ToString();
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =workbook.Worksheets.Add("Authors");
                    worksheet.Cell(1, 1).Value = "Id";
                    worksheet.Cell(1, 2).Value = "Name";
                    worksheet.Cell(1, 3).Value = "Price";

                    worksheet.Cell(1, 4).Value = "LastUpdated";
                    var product = GetProducts(query);
                    for (int index = 1; index <= product.Count; index++)
                    {
                        worksheet.Cell(index + 1, 1).Value = index;
                        worksheet.Cell(index + 1, 2).Value =
                        product[index - 1].Name;
                        worksheet.Cell(index + 1, 3).Value =
                        product[index - 1].Price;
                        worksheet.Cell(index + 1, 3).Value =
                       product[index - 1].LastUpdated;
                    }
                    var filepath = HttpContext.Current.Server.MapPath("~/File/" );
                    workbook.SaveAs(filepath + fileName+".xlsx") ;
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return fileName;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // POST: api/Product

        [HttpPost]
        [ResponseType(typeof(Product))]
        public HttpResponseMessage PostProduct()
        {
            HttpResponseMessage result = new HttpResponseMessage();
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            var HttpBody = HttpContext.Current.Request.Form["product"];
            HttpBody = HttpBody.Replace("null", "0");
            Product product = JsonConvert.DeserializeObject<Product>(HttpBody);
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    
                    imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    imageName = imageName + DateTime.UtcNow.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filepath = HttpContext.Current.Server.MapPath("~/Image/" + imageName);
                    postedFile.SaveAs(filepath);

                    product.Photo = imageName;
                    product.LastUpdated = DateTime.UtcNow;
                    _productRepository.Add(product);
                    _productRepository.Commit();
                }
            }
            result.StatusCode = HttpStatusCode.OK;

            return result;
        }
        // DELETE: api/Product/5
        [System.Web.Http.HttpDelete]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = _productRepository.First(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _productRepository.Delete(product);
            _productRepository.Commit();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _productRepository.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return _productRepository.Any(p => p.Id == id);
        }

    }
}