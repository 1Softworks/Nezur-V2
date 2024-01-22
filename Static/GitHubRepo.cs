using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Controls;

using Newtonsoft.Json;

namespace NezurAimbot
{
    public class GitHubRepo
    {
        private const string GitHubAPI = "https://api.github.com/repos";
        private readonly HttpClient httpClient;

        public GitHubRepo()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Nezur");
        }

        public async Task<string?> DownloadFile(string owner, string repo, string filePath)
        {
            string apiUrl = $"{GitHubAPI}/{owner}/{repo}/contents/{filePath}";
            string responseJson = await httpClient.GetStringAsync(apiUrl);
            var fileDetails = JsonConvert.DeserializeObject<GitHubFileDetails>(responseJson);

            if (fileDetails?.Type == "file")
            {
                return await httpClient.GetStringAsync(fileDetails.DownloadUrl);
            }

            return null;
        }

        public async Task<bool> DownloadFiles(string owner, string repo, string folderPath, string destinationPath)
        {
            string apiUrl = $"{GitHubAPI}/{owner}/{repo}/contents/{folderPath}";
            string responseJson = await httpClient.GetStringAsync(apiUrl);
            var folderContents = JsonConvert.DeserializeObject<GitHubContents[]>(responseJson);

            if (folderContents != null)
            {
                foreach (var file in folderContents)
                {
                    if (file.Type == "file")
                    {
                        byte[] fileBytes = await httpClient.GetByteArrayAsync(file.DownloadUrl);

                        string fileName = Path.GetFileName(file.Name) ?? "UnknownFile";
                        string destinationFilePath = Path.Combine(destinationPath, fileName);

                        await File.WriteAllBytesAsync(destinationFilePath, fileBytes);
                    }
                }

                return true;
            }

            return false;
        }

        private class GitHubFileDetails
        {
            [JsonProperty("type")]
            public string? Type { get; set; }

            [JsonProperty("download_url")]
            public string? DownloadUrl { get; set; }
        }

        private class GitHubContents
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("type")]
            public string? Type { get; set; }

            [JsonProperty("download_url")]
            public string? DownloadUrl { get; set; }
        }
    }
}