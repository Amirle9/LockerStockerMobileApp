using Android.Hardware.Camera2;
using Android.Util;
using Azure.Storage.Blobs;
using System;
using System.IO;

namespace Camera2Basic.Listeners
{
    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private static readonly string TAG = "CameraCaptureStillPictureSessionCallback";

        private readonly Camera2BasicFragment owner;

        public CameraCaptureStillPictureSessionCallback(Camera2BasicFragment owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            // If something goes wrong with the save (or the handler isn't even 
            // registered, this code will toast a success message regardless...)
            owner.ShowToast("Saved: " + owner.mFile);
            Log.Debug(TAG, owner.mFile.ToString());
            Byte[] bytes = File.ReadAllBytes("/storage/emulated/0/Android/data/Camera2Basic.Camera2Basic/files/pic.jpg");
             String file = Convert.ToBase64String(bytes);
             Log.Debug(TAG,file);
            uploadFile("/storage/emulated/0/Android/data/Camera2Basic.Camera2Basic/files/pic.jpg");
            var results = AzureApi.sendPhotoNotofication(azureClient.LockerId);
            owner.UnlockFocus();
        }

        public static async void uploadFile(string path)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountdefau8140;AccountKey=yCQK9mj77GChmhG5Ghe4cyA5ftIMiWZtm/Jg/6W8jMtBUdmoIhuLDEjllq9JCIK5o6XeNWWcfL/vOHWtNX8WKw==;EndpointSuffix=core.windows.net";
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            //Create a unique name for the container
            string containerName = "lockerphotos";
            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // Get a reference to a blob
            BlobClient blobClientUpload = containerClient.GetBlobClient("LockerStocker_viaCamera_" + azureClient.LockerId + ".jpeg");
            //FileStream upFileStream = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            FileStream upFileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            //upFileStream = File.OpenRead(imagePath);
            await blobClientUpload.UploadAsync(upFileStream, true);
            upFileStream.Close();
        }
    }
}
