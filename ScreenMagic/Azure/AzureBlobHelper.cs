using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ScreenMagic
{
    class AzuerBlobHelper
    {
        //URI format: https://gerhastest.blob.core.windows.net/gerhascontainer/test.txt
        static CloudStorageAccount storageAccount = null;
        static CloudBlobContainer cloudBlobContainer = null;

        private static string CONTAINER_ID = "gerhascontainer";

        public static void Setup()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");

            // Check whether the connection string can be parsed.
            CloudStorageAccount.TryParse(storageConnectionString, out storageAccount);
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            cloudBlobContainer = cloudBlobClient.GetContainerReference(CONTAINER_ID);
        }

        public async static void Delete()
        {
            await cloudBlobContainer.DeleteIfExistsAsync();
        }

        public static async Task InitFirst()
        {
            // Retrieve the connection string for use with the application. The storage connection string is stored
            // in an environment variable on the machine running the application called storageconnectionstring.
            // If the environment variable is created after the application is launched in a console or with Visual
            // Studio, the shell needs to be closed and reloaded to take the environment variable into account.
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.

            // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
            await cloudBlobContainer.CreateIfNotExistsAsync();
            Debug.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
            Debug.WriteLine("");

            // Set the permissions so the blobs are public. 
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await cloudBlobContainer.SetPermissionsAsync(permissions);
        }

        public static async Task<string> UploadFile(byte[] content, string filename)
        {

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
            await cloudBlockBlob.UploadFromByteArrayAsync(content, 0, content.Length).ConfigureAwait(false);
                        return GetHttpUriFromFileName(filename);
        }

        public static string GetHttpUriFromFileName(string filename)
        {
            return @"https://gerhastest.blob.core.windows.net/gerhascontainer/" + filename;
        }
    }
}
