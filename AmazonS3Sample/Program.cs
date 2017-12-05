using System;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;

namespace AmazonS3Sample
{
    class Program
    {
        private const string AccessKeyID = "AKIAJVSYPSVAC6BPILKQ";
        private const string SecretAccessKeyID = "LidX+kJlcqcmHdamw7BlsOaljuXdJNSDFp8IMoOu";
        private const string filePath = "C:\\Users\\ramra\\Downloads\\aws-lambda-ffmpeg-develop\\aws-lambda-ffmpeg-develop\\test\\fixtures\\good.mp4";
        private const string s3Bucket = "ramraj-s3-testfolder/FromVideoPath";
        private static string serviceUrl = "https://ap-south-1.console.aws.amazon.com";  //N. Virginia service url         
        private static string newFileName = "Good-" + DateTime.Now.Ticks.ToString() + ".mp4"; //new filename in s3, optional
       

        private const string BUCKET_NAME = "S3DemoBucket";
        private const string S3_KEY = "s3_key";
        static void Main(string[] args)
        {
            AccessKeyID.Trim();
            SecretAccessKeyID.Trim();
            IAmazonS3 s3Client = GetS3Client();
            //CreateBucket(s3Client);
            var File = GetFile(s3Client);

        }
        public static IAmazonS3 GetS3Client()
        {
            IAmazonS3 s3Client = new AmazonS3Client(AccessKeyID, SecretAccessKeyID, Amazon.RegionEndpoint.APSouth1);
            return s3Client;
        }
        /// <summary>
        /// Creates bucket if it is not exists.
        /// </summary>
        /// <param name="client"></param>
        private static void CreateBucket(IAmazonS3 client)
        {
            Console.Out.WriteLine("Checking S3 bucket with name " + BUCKET_NAME);
            ListBucketsResponse response = client.ListBuckets();
            bool found = false;
            foreach (S3Bucket bucket in response.Buckets)
            {
                if (bucket.BucketName == BUCKET_NAME)
                {
                    Console.Out.WriteLine("   Bucket found will not create it.");
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                Console.Out.WriteLine("   Bucket not found will create it.");
                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = s3Bucket,
                    FilePath = filePath,
                    Key = S3_KEY,
                    CannedACL = Amazon.S3.S3CannedACL.PublicRead
                };

                // Put object
                PutObjectResponse responsePut = client.PutObject(request);

              //  client.PutBucket(new PutBucketRequest().WithBucketName(BUCKET_NAME));

                Console.Out.WriteLine("Created S3 bucket with name " + BUCKET_NAME);
            }

        }
        public static MemoryStream GetFile(IAmazonS3 s3Client)
        {
            using (s3Client)
            {
                MemoryStream file = new MemoryStream();
                try
                {
                    GetObjectResponse r = s3Client.GetObject(new GetObjectRequest()
                    {
                        BucketName = s3Bucket,
                        Key = S3_KEY
                    });
                    try
                    {
                        long transferred = 0L;
                        BufferedStream stream2 = new BufferedStream(r.ResponseStream);
                        byte[] buffer = new byte[0x2000];
                        int count = 0;
                        while ((count = stream2.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            file.Write(buffer, 0, count);
                        }
                    }
                    finally
                    {
                    }
                    return file;
                }
                catch (AmazonS3Exception)
                {
                    //Show exception
                }
            }
            return null;
        }


    }
}
