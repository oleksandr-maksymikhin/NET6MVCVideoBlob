using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace NET6MVCVideoBlob.Controllers
{
    public class VideosController : Controller 
    {
        BlobServiceClient serviceClient;
        string blobContainerName = "test-net6-video-blob";

        public VideosController(BlobServiceClient serviceClient)
        {
            this.serviceClient = serviceClient;
        }

        // GET: VideosController
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: Videos/Get View with <video> that call the method - ActionResult GetVideo()
        public IActionResult VideoPlayer()
        {
            return View();
        }

        public async Task<ActionResult>  GetVideo()
        {
            try
            {
                //var stream = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
                BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(blobContainerName);
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    Stream stream = await blobClient.OpenReadAsync();
                    //// return status code 200
                    //var response = new FileStreamResult(stream, "video/mp4");
                    //// return status code 206
                    var response = File(stream, "video/mp4", enableRangeProcessing: true);
                    return response;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error or return a custom error view.
                return View("Error");
            }
            return RedirectToAction(nameof(Create));
        }

        // GET: VideosController/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST:VideosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormFile video)
        {
            await AddVideo(video);
            return RedirectToAction(nameof(Create));
        }

        private async Task AddVideo(IFormFile video)
        {
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(blobContainerName);
            bool isExist = await containerClient.ExistsAsync();
            if (!isExist) 
            {
                containerClient = await serviceClient.CreateBlobContainerAsync(blobContainerName);
            }
            string blobName = video.FileName;
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using(var fileStream = video.OpenReadStream())
            {
                await blobClient.UploadAsync(fileStream, true);
            }
        }
    }
}
