using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using SwaggerDemo.Models;

namespace SwaggerDemo.Controllers
{
    [ApiController]
    [Route("/")]
    public class AzureController : ControllerBase
    {
        private DBContext _context;
        private IConfiguration _configuration;
        private readonly string _azureStorageconnectionString;
        private readonly ILogger<AzureController> _logger;
        public AzureController(IConfiguration configuration, ILogger<AzureController> logger)
        {
            _configuration = configuration;
            _context = new DBContext(_configuration);
            _azureStorageconnectionString = _configuration.GetConnectionString("AzureStorageConn");
            _logger = logger;
        }

        //[HttpGet]
        //[Route("GetFilesToUploadToBlob")]
        //public IActionResult GetFilesToUploadToBlob()
        //{
        //    _context.UploadBlob();
        //    return Ok("File Uploaded to Blob Storage");
        //}

        [HttpGet]
        [Route("DownloadFileFromAzureBlob")]
        public async Task<IActionResult> DownloadFileFromAzureBlob(string fileName)
        {
            CloudBlockBlob blockBlob;
            await using (MemoryStream memoryStream = new MemoryStream())
            {
                string blobstorageconnection = _azureStorageconnectionString;
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_configuration.GetValue<string>("BlobContainerName"));
                blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                await blockBlob.DownloadToStreamAsync(memoryStream);
            }
            Stream blobStream = blockBlob.OpenReadAsync().Result;
            return File(blobStream, blockBlob.Properties.ContentType, blockBlob.Name);
        }

        [HttpGet]
        [Route("LogAppInSights")]
        public IActionResult LogAppInSights()
        {
            //text message
            var iteration = 1;
            _logger.LogDebug($"Debug {iteration}");
            _logger.LogInformation($"Information {iteration}");
            _logger.LogWarning($"Warning {iteration}");
            _logger.LogError($"Error {iteration}");
            _logger.LogCritical($"Critical {iteration}");
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return Ok("Data logged in App Insights");
        }

        [HttpGet]
        [Route("ReadAppConfig")]
        public string ReadAppConfig()
        {
            return _configuration.GetValue<string>("TestAppConfigKey");
        }

        [HttpGet]
        [Route("GetSecretFromKeyVault")]
        public string GetSecretFromKeyVault()
        {
            var value = _configuration["SampleSecret"];
            return "Value for Secret [SampleSecret] is : " + value;
        }
    }
}
