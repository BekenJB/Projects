using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace UploadToAzureContainer.Controllers
{
    public class UploadFilesController : Controller
    {
        private readonly IConfiguration configuration;
        public UploadFilesController(IConfiguration configs)
        {
            this.configuration = configs;
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            //reading the connection string from the appSetting
            string connectionString = configuration.GetConnectionString("StorageConnectionString");

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("categories");

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();

            await container.SetPermissionsAsync(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            long size = files.Sum(f => f.Length);
           
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string key = DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm-ss") + "-" + formFile.FileName;
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(key);

                  
                        await blockBlob.UploadFromStreamAsync(formFile.OpenReadStream());                    
                }
            }

            return Ok(new { count = files.Count, size, isSuccessful=true });
        }

        // GET: UploadFiles
        public ActionResult Index()
        {
            return View();
        }

        // GET: UploadFiles/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UploadFiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UploadFiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UploadFiles/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UploadFiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UploadFiles/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UploadFiles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}