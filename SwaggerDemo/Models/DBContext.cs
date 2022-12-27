using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Validations;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SwaggerDemo.Models
{
    public class DBContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _sqlconnectionString;
        private readonly string _azureStorageconnectionString;
        public DBContext(IConfiguration configuration)
        {
            _configuration = configuration;
            //_sqlconnectionString = _configuration.GetConnectionString("MyConn");
            _sqlconnectionString = _configuration.GetConnectionString("SQLDBConn");
            _azureStorageconnectionString = _configuration.GetConnectionString("AzureStorageConn");
        }

        public List<Customer> GetCustomers()
        {
            List<Customer> customers;
            using (SqlConnection sqlConnection = new SqlConnection(_sqlconnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetCustomers", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlConnection.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                customers = new List<Customer>();
                while (sdr.Read())
                {
                    customers.Add(new Customer()
                    {
                        Id = Convert.ToInt32(sdr["ID"]),
                        Name = sdr["Name"].ToString(),
                        Age = Convert.ToInt32(sdr["Age"]),
                        Department = sdr["DepartmentName"].ToString(),
                        DateOfBirth = Convert.ToDateTime(sdr["DateOfBirth"])
                    });
                }
            }
            return customers;
        }

        public void UploadBlob()
        {
            string containerName = "blobcontainer";
            var serviceClient = new BlobServiceClient(_azureStorageconnectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);
            var path = @"C:\Users\enjoy\OneDrive\Desktop\Raji\DOTNET\Azure";
            var fileName = "Testfile3.txt";
            var localFile = Path.Combine(path, fileName);
            File.AppendAllText(localFile, "This is a test message");
            var blobClient = containerClient.GetBlobClient(fileName);
            Console.WriteLine("Uploading to Blob storage");
            using FileStream uploadFileStream = File.OpenRead(localFile);
            blobClient.Upload(uploadFileStream);
            uploadFileStream.Close();
        }
    }
}
