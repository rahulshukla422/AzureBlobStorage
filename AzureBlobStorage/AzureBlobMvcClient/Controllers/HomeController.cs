using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AzureBlobStorage;
using System.Threading.Tasks;

namespace AzureBlobMvcClient.Controllers
{
    public class HomeController : Controller
    {
        BlobStorage storage;
        // GET: Home
        public ActionResult Index()
        {
            storage = new BlobStorage();

            var response = storage.GetStorage();

            return View(response);
        }
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase photo)
        {
            storage = new BlobStorage();
            await storage.UploadBlob(photo);

            return RedirectToAction("Index");
        }

        public void Download(string path)
        {
            storage = new BlobStorage();

            storage.DownloadBlob(path);

        }
        public async Task<ActionResult> Delete(string path)
        {
            storage = new BlobStorage();

            bool success = await storage.DeleteBlob(path);

            if (success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }

        }
    }
}