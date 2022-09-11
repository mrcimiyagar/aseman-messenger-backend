using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace HostVersion.Utils
{
    public class HttpUploader
    {
        public static void HttpUploadFile(string url, string file, string paramName, string contentType,
            NameValueCollection nvc)
        {
            Console.WriteLine($"Uploading {file} to {url}");
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest) WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            var rs = wr.GetRequestStream();

            var formDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundaryBytes, 0, boundaryBytes.Length);
                string formItem = string.Format(formDataTemplate, key, nvc[key]);
                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(formItem);
                rs.Write(formItemBytes, 0, formItemBytes.Length);
            }

            rs.Write(boundaryBytes, 0, boundaryBytes.Length);

            const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerBytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerBytes, 0, headerBytes.Length);

            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            fileStream.Close();

            var trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wResp = null;
            try
            {
                wResp = wr.GetResponse();
                var stream2 = wResp.GetResponseStream();
                var reader2 = new StreamReader(stream2 ?? throw new InvalidOperationException());
                Console.WriteLine($"File uploaded, server response is: {reader2.ReadToEnd()}");
            }
            catch (Exception)
            {
                Console.WriteLine("Error uploading file");
                wResp?.Close();
            }
        }
    }
}