using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenAI
{
    public class ChatAPI
    {
        public static T DeserializeObject<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
        public static string SerializeObject(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        public string BaseUrl { get; set; } = "https://open.bigmodel.cn/api/paas/v4/";
        public string APIKey { get; set; } = "";
        public ChatAPI() { }
        public ChatAPI(string baseUrl, string apiKey)
        {
            BaseUrl = baseUrl;
            APIKey = apiKey;
        }
        public async Task<ModelList> GetModels(CancellationToken cancellationToken = new CancellationToken())
        {
            string url = BaseUrl.TrimEnd('/') + "/models";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", APIKey);
                client.DefaultRequestHeaders.Add("api-key",APIKey);
                HttpResponseMessage httpResponse = await client.GetAsync(url, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var response = await httpResponse.Content.ReadAsStringAsync();
                        return DeserializeObject<ModelList>(response);
                    }
                }
                return new ModelList();
            }
        }
        public async Task<EmbeddingResponse> GetEmbeddings(EmbeddingRequest req, CancellationToken cancellationToken = new CancellationToken())
        {
            string url = BaseUrl.TrimEnd('/') + "/embeddings";
            using (HttpClient client = new HttpClient())
            {
                string json = SerializeObject(req);
                HttpContent content = new StringContent(json);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", APIKey);
                client.DefaultRequestHeaders.Add("api-key",APIKey);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage httpResponse = await client.PostAsync(url, content, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var response = await httpResponse.Content.ReadAsStringAsync();
                        return DeserializeObject<EmbeddingResponse>(response);
                    }
                }
                return new EmbeddingResponse();
            }
        }
        public async Task<ChatResponse> Chat<T>(ChatRequest<T> req, CancellationToken cancellationToken = new CancellationToken())
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseUrl.TrimEnd('/') + "/chat/completions";
                req.stream = false;
                string json = SerializeObject(req);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", APIKey);
                client.DefaultRequestHeaders.Add("api-key",APIKey);
                HttpResponseMessage httpResponse = await client.PostAsync(url, content, cancellationToken);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string str = await httpResponse.Content.ReadAsStringAsync();
                    return DeserializeObject<ChatResponse>(str);
                }
                throw new Exception(httpResponse.ToString());
            }
        }
        public async IAsyncEnumerable<ChatResponse> ChatStream<T>(ChatRequest<T> req, CancellationToken cancellationToken = new CancellationToken())
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseUrl.TrimEnd('/') + "/chat/completions";
                req.stream = true;
                string json = SerializeObject(req);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", APIKey);
                client.DefaultRequestHeaders.Add("api-key",APIKey);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
                HttpResponseMessage httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        using (var responseStream = await httpResponse.Content.ReadAsStreamAsync())
                        {
                            if (responseStream.CanRead)
                            {
                                using (StreamReader reader = new StreamReader(responseStream))
                                {
                                    while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
                                    {
                                        string line = await reader.ReadLineAsync();
                                        string head = "data:";
                                        if (line.IndexOf(head) == 0)
                                        {
                                            string data = line.Substring(head.Length);
                                            if (Regex.IsMatch(data, "\\s?\\[DONE\\]"))
                                            {
                                                yield break;
                                            }
                                            else
                                            {
                                                ChatResponse rep = DeserializeObject<ChatResponse>(data);
                                                yield return rep;
                                            }
                                        }
                                    }

                                }
                            }
                            throw new Exception(httpResponse.ToString());
                        }
                    }
                    throw new Exception(httpResponse.ToString());
                }
            }
        }
        public async Task<ImageResponse> ImageGen(ImageRequest req, CancellationToken cancellationToken = new CancellationToken())
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseUrl.TrimEnd('/') + "/images/generations";
                string json = SerializeObject(req);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", APIKey);
                client.DefaultRequestHeaders.Add("api-key",APIKey);
                HttpResponseMessage httpResponse = await client.PostAsync(url, content, cancellationToken);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string str = await httpResponse.Content.ReadAsStringAsync();
                    return DeserializeObject<ImageResponse>(str);
                }
                throw new Exception(httpResponse.ToString());
            }
        }
    }
}
