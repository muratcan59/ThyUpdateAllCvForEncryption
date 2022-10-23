using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ThyUpdateAllCvForEncryption
{
    internal static class EncryptionService
    {
        public static string Encryption(string data)
        {
            if (String.IsNullOrEmpty(data)) return null;

            try
            {
                using (var client = new HttpClient())
                {

                    ThyEncryptModel encryptModel = new ThyEncryptModel
                    {
                        dataIn = data
                    };
                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false,true);
                    IConfiguration config = builder.Build();
                    var uri = config.GetSection("ThyApiEndpoint")["Endpoint"];
                    var userName = config.GetSection("ThyBasicAuthInformation")["UserName"];
                    var password = config.GetSection("ThyBasicAuthInformation")["Password"];
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Clear();
                    var authenticationString = $"{userName}:{password}";
                    var encodedAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",encodedAuth);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var serializeData = JsonConvert.SerializeObject(encryptModel);
                    var stringContent = new StringContent(serializeData, Encoding.UTF8);
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post,uri);
                    req.Content = new StringContent(serializeData, Encoding.UTF8, "application/json");
                    var res = client.SendAsync(req).Result;
                    if (res.IsSuccessStatusCode)
                    {
                        var response = res.Content.ReadAsStringAsync().Result;
                        var deserializedData = JsonConvert.DeserializeObject(response);
                        return (string)deserializedData;
                    }
                    return null;

                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
