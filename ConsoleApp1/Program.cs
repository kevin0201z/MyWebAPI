using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            //string result1 = HttpGet();
            //string result2 = HttpGetWithPara("有参方法");
            //string result3 = HttpPost("post");
            PostClient();

            //GetClient();

            //GetParaClient("2333");
            //Console.WriteLine(result1);
            //Console.WriteLine(result2);

            Console.ReadKey();

        }

        #region HttpWebRequest调用API接口
        /// <summary>
        /// 通过HttpWebRequest调用无参的Get方法
        /// </summary>
        static string HttpGet()
        {
            string retStr = "";
            HttpWebRequest request = null;
            try
            {
                string fullUrl = "http://localhost:38149/api/TestHttp";
                request = (HttpWebRequest)HttpWebRequest.Create(fullUrl);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 3600;
                request.ReadWriteTimeout = 3600;
                retStr = ReadStringResponse(request.GetResponse());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retStr;
        }

        /// <summary>
        /// 通过HttpWebRequest调用有参数的Get方法
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        static string HttpGetWithPara(string para)
        {
            string retStr = "";
            HttpWebRequest request = null;
            try
            {
                string fullUrl = $"http://localhost:38149/api/TestHttp/{para}";
                request = (HttpWebRequest)HttpWebRequest.Create(fullUrl);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 3600;
                request.ReadWriteTimeout = 3600;
                retStr = ReadStringResponse(request.GetResponse());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retStr;
        }

        /// <summary>
        /// 解析web响应，以string形式返回
        /// </summary>
        /// <param name="response">web响应</param>
        /// <returns>返回string形式的web响应</returns>
        private static String ReadStringResponse(WebResponse response)
        {
            string returnStr = "";
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            returnStr = sr.ReadToEnd();
            sr.Close();
            return returnStr;
        }

        /// <summary>
        /// 通过HttpWebRequest调用Post方法
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        static string HttpPost(string body)
        {
            string url = "http://localhost:38149/api/TestHttp";
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";

            byte[] buffer = encoding.GetBytes(body);

            request.ContentLength = buffer.Length;


            Stream stream = request.GetRequestStream();
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                return reader.ReadToEnd();
            }
        }
        #endregion

        #region HttpClient调用API接口
        /// <summary>
        /// 通过HttpClient调用Get方法
        /// </summary>
        static async void GetClient()
        {
            string url = "http://localhost:38149/api/TestHttp";

            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);//改成自己的

                    response.EnsureSuccessStatusCode();//用来抛异常的
                    string responseBody = await response.Content.ReadAsStringAsync();
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 通过HttpClient调用有参的Get方法
        /// </summary>
        /// <param name="para"></param>
        static async void GetParaClient(string para)
        {
            string strReturn = "";
            string url = "http://localhost:29693/api/WeatherForecast";

            try
            {
                using (var client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + para);//token验证方法1
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", para);//token验证方法2
                    HttpResponseMessage response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();//用来抛异常的
                    strReturn = await response.Content.ReadAsStringAsync();
                }

            }
            catch (Exception ex)
            {
                strReturn = ex.Message;
            }
        }

        /// <summary>
        /// 通过HttpClient调用Post方法
        /// </summary>
        static async void PostClient()
        {
            string url = "http://localhost:29693/api/Authenticate/login";

            try
            {
                var str = JsonConvert.SerializeObject(new LoginInput() { 
                    Username = "abc",
                    Password = "123" 
                });

                HttpContent content = new StringContent(str);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);//改成自己的                  
                    string responseBody = await response.Content.ReadAsStringAsync();
                    //response.EnsureSuccessStatusCode();//用来抛异常的
                    if (response.IsSuccessStatusCode)
                    {
                        sToken token = JsonConvert.DeserializeObject<sToken>(responseBody);
                        var t = token.token;
                        GetParaClient(t);
                    }
                    else
                        Console.WriteLine(response.StatusCode.ToString() + ":" + responseBody);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion
    }
}