using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web;
using System.IO;
using System.Net;

namespace AzureBlobStorage
{
    public class BlobStorage
    {
        IList<string> list = new List<string>();
        CloudStorageAccount storageAccount;

        public IEnumerable<string> GetStorage()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference("images");

            if (container.Exists())
            {
                IEnumerable<IListBlobItem> itemes = container.ListBlobs();

                foreach (var item in itemes)
                {
                    // Console.WriteLine(item.Uri.AbsoluteUri);
                    list.Add(item.Uri.AbsoluteUri);
                }

            }
            return list;
        }
        public async Task<string> UploadBlob(HttpPostedFileBase imageToUpload)
        {
            string imageFullPath = null;

            if (imageToUpload == null | imageToUpload.ContentLength == 0)
                return null;

            try
            {
                storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                CloudBlobClient client = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = client.GetContainerReference("images");

                string imageName = $"{Guid.NewGuid().ToString()}-{Path.GetExtension(imageToUpload.FileName)}";

                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(imageName);

                cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;

                await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.InputStream);

                imageFullPath = cloudBlockBlob.Uri.ToString();

            }
            catch (Exception ex)
            {

                throw;
            }

            return imageFullPath;
        }

        public void DownloadBlob(string path)
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference("images");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(Path.GetFileName(path));

            using (MemoryStream ms = new MemoryStream())
            {
                blockBlob.DownloadToStream(ms);

                HttpContext.Current.Response.ContentType = blockBlob.Properties.ContentType.ToString();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "Attachment; filename=" + Path.GetFileName(path).ToString());
                HttpContext.Current.Response.AddHeader("Content-Length", blockBlob.Properties.Length.ToString());
                HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
            }
        }
        public async Task<bool> DeleteBlob(string fileToDelete)
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference("images");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(Path.GetFileName(fileToDelete));

            if (blockBlob.Exists())
            {
                await blockBlob.DeleteAsync();

                return true;
            }
            return false;

        }
    }
}

