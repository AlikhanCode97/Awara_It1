using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AwaraIT.BCS.ConsoleApp.Actions
{
    internal static class TestAction
    {
        private static readonly string flowUrl =
            "https://4bdff9edf2e9ef33ba314ad199dd94.17.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/309f51f6ca33492dae6724499e388596/triggers/manual/paths/invoke?api-version=1&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=RCdsWDr5htYmoZ0jglL82eU526Nunwd-S5RUC00Dznw";


        // -------------------------------
        // FILENAME EXTRACTION FUNCTION
        // -------------------------------
        private static string ExtractSafeFilename(string fileId)
        {
            // decode twice because SharePoint stores %25d0%25xx patterns
            string decoded = Uri.UnescapeDataString(fileId);
            decoded = Uri.UnescapeDataString(decoded);

            // extract filename after last slash
            string filename = decoded.Substring(decoded.LastIndexOf('/') + 1);

            // enforce CRM limit 255 characters
            if (filename.Length > 255)
            {
                string ext = "";
                int dotIdx = filename.LastIndexOf('.');
                if (dotIdx > 0)
                    ext = filename.Substring(dotIdx);

                string baseName = filename.Substring(0, 255 - ext.Length);
                filename = baseName + ext;
            }

            return filename;
        }


        public static void Run(Guid emailId)
        {
            using (var service = Program.GetCrmClient())
            {
                Console.WriteLine($"Retrieving email: {emailId}");

                // Retrieve email
                var email = service.Retrieve(
                    "email",
                    emailId,
                    new ColumnSet("subject", "awr_sharepointfiles")
                );

                string json = email.GetAttributeValue<string>("awr_sharepointfiles");
                if (string.IsNullOrEmpty(json))
                {
                    Console.WriteLine("No awr_sharepointfiles found on email.");
                    return;
                }

                List<string> fileIds = JsonSerializer.Deserialize<List<string>>(json);
                if (fileIds == null || fileIds.Count == 0)
                {
                    Console.WriteLine("FileId list is empty.");
                    return;
                }

                Console.WriteLine($"Found {fileIds.Count} files.");

                using (var http = new HttpClient())
                {
                    foreach (var fileId in fileIds)
                    {
                        try
                        {
                            Console.WriteLine($"Processing fileId: {fileId}");

                            // Call Flow
                            var payload = new { fileId };
                            var bodyJson = JsonSerializer.Serialize(payload);
                            var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                            var resp = http.PostAsync(flowUrl, content).Result;

                            if (!resp.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Flow returned {resp.StatusCode} for {fileId}");
                                continue;
                            }

                            // bytes = actual file content (binary)
                            var bytes = resp.Content.ReadAsByteArrayAsync().Result;

                            // convert to base64 for CRM attachment
                            string base64 = Convert.ToBase64String(bytes);

                            // MIME type
                            string mime = resp.Content.Headers.ContentType?.MediaType
                                          ?? "application/octet-stream";

                            // decode filename safely
                            string filename = ExtractSafeFilename(fileId);

                            Console.WriteLine($"Saving attachment as: {filename}");

                            // Create CRM attachment
                            var attach = new Entity("activitymimeattachment");
                            attach["activityid"] = email.ToEntityReference();
                            attach["filename"] = filename;
                            attach["body"] = base64;
                            attach["mimetype"] = mime;

                            service.Create(attach);

                            Console.WriteLine($"Attachment {filename} attached successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ERROR processing fileId {fileId}: {ex}");
                        }
                    }
                }

                Console.WriteLine("Completed.");
            }
        }
    }
}
