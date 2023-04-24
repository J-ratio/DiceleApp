using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;


[Serializable]
public class ApiResponse
{
    public string response { get; set; }
    public HttpRequestException error { get; set; }

    public ApiResponse()
    {
        response = null;
        error = null;
    }

}

public static class ApiHelper
{
    public static async Task<ApiResponse> SendJSONData(string uri, object json)
    {
        try
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonUtility.ToJson(json), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(uri, content);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            ApiResponse result = new ApiResponse();
            result.response = responseBody;
            return result;
        }
        catch (HttpRequestException error)
        {
            ApiResponse result = new ApiResponse();
            result.error = error;
            return result;
        }
    }

    public static async Task<ApiResponse> RequestJSONData(string uri)
    {
        try
        {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(uri);

            ApiResponse result = new ApiResponse();
            result.response = response;
            return result;
        }
        catch (HttpRequestException error)
        {
            ApiResponse result = new ApiResponse();
            result.error = error;
            return result;
        }
    }

    public static void DownloadFile(string imageFullUri, string folderNameToSave, string fileNameToSave)
    {
        Directory.CreateDirectory(folderNameToSave);

        try
        {
            Uri uri = new Uri(imageFullUri);
            WebClient webClient = new WebClient();
            webClient.DownloadFile(uri, folderNameToSave + fileNameToSave);
        }
        catch (WebException error) { }
    }

    public static void DownloadFileAsync(string imageFullUri, string folderNameToSave, string fileNameToSave)
    {
        Directory.CreateDirectory(folderNameToSave);

        Uri uri = new Uri(imageFullUri);
        WebClient webClient = new WebClient();
        webClient.DownloadFileAsync(uri, folderNameToSave + fileNameToSave);
    }
}
