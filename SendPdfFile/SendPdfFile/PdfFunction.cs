using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SendPdfFile
{
    public class PdfFunction
    {
        [FunctionName("PdfFile")]
        public async Task<FileContentResult> PdfFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdffile")] HttpRequest req)
        {
            try
            {
                var fileName = "SampleFile.pdf";
                BlobServiceClient storageAccount = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                BlobContainerClient container = storageAccount.GetBlobContainerClient("pdf");
                container.CreateIfNotExists(PublicAccessType.Blob);

                BlockBlobClient blockBlob = container.GetBlockBlobClient(fileName);

                MemoryStream ms = new MemoryStream();

                await blockBlob.DownloadToAsync(ms);

                return new FileContentResult(ms.ToArray(), "application/pdf");
            }
            catch (Exception ex)
            {
                return new FileContentResult(new byte[0], "application/pdf");
            }
        }

        [FunctionName("PdfResponse")]
        public async Task<HttpResponseMessage> PdfResponse(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdfresponse")] HttpRequest req)
        {
            try
            {
                var fileName = "SampleFile.pdf";

                BlobServiceClient storageAccount = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                BlobContainerClient container = storageAccount.GetBlobContainerClient("pdf");
                container.CreateIfNotExists(PublicAccessType.Blob);

                BlockBlobClient blockBlob = container.GetBlockBlobClient(fileName);

                MemoryStream ms = new MemoryStream();

                await blockBlob.DownloadToAsync(ms);


                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Content = new ByteArrayContent(ms.ToArray());
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                {
                    FileName = fileName
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
