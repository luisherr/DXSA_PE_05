using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DXSA_PE_05.Tools
{
    public class Email  
    {
        MailMessage oMailMessage = new MailMessage  ();
        SmtpClient  oSmtpClient = new SmtpClient   ();

        public void from(String from)
        {
            oMailMessage.From = new MailAddress(from);
        }

        public void to(String to)
        {
            foreach(var To in to.Split(';'))
            {
                oMailMessage.To.Add(new MailAddress(To));
            }
        } 

        public void subject(String subject)
        {
            oMailMessage.Subject = subject;
            oMailMessage.Priority = MailPriority.Normal;
        }

        public void body(String body)
        {
            oMailMessage.Body = body;
            oMailMessage.IsBodyHtml = true;
        }

        public void host(String host)
        {
            oSmtpClient.Host = host;
        }

        public void port(String port)
        {
            oSmtpClient.Port = Convert.ToInt32(port);
        }

        public void enableSsl(int enableSsl)
        {
            if(enableSsl == 0)
            {
                oSmtpClient.EnableSsl = false;
            }
            else
            {
                oSmtpClient.EnableSsl = true;
            }
        }

        public void Attachment(System.IO.Stream contentStream, String name)
        {
            oMailMessage.Attachments.Add(new Attachment(contentStream, name));
        }

        public void EnviarCorreo(String password, String username)
        {
            oSmtpClient.UseDefaultCredentials = false;
            oSmtpClient.Credentials = new NetworkCredential(username, password);

            oSmtpClient .Send(oMailMessage);
            oMailMessage.Dispose();
            oSmtpClient .Dispose();
        }
    }
    public class API
    {

        public Boolean httpEvent = false;
        public String Message = String.Empty;
        public String ContentType = String.Empty;
        public dynamic Response;

        public void Post(string URL, string rawText, dynamic _Response)
        {

            HttpClient http = new HttpClient();
            http.BaseAddress = new Uri(URL);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
            http.Timeout = TimeSpan.FromSeconds(9999);

            StringContent oString = new StringContent(rawText, Encoding.UTF8, ContentType);

            HttpResponseMessage response = null;
            String respuesta = String.Empty;

            try
            {
                Task.Run(async () =>
                {
                    response = await http.PostAsync(URL, oString);
                    respuesta = await response.Content.ReadAsStringAsync();
                }).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error de comunicación con {URL}, status code: {response.StatusCode}");
                }

                _Response = JsonConvert.DeserializeObject(respuesta, _Response.GetType());

                Response = _Response;

                if (Response.codEstado == "1")
                {
                    httpEvent = true;
                }
                else
                {
                    httpEvent = false;
                }
            }

            catch (Exception ex)
            {
                Message = $"Error al consumir el método POST, mas detalle: {ex.Message}";
                httpEvent = false;
            }

        }

        public void Post(String API, String URI, dynamic _Request, dynamic _Response)
        {
            String JsonRequest = JsonConvert.SerializeObject(_Request);

            HttpClient http = new HttpClient();

            StringContent oString = new StringContent(JsonRequest);
            oString.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = null;
            var respuesta = String.Empty;

            try
            {
                Task.Run(async () =>
                {
                    response = http.PostAsync((API + URI), oString).Result;
                    respuesta = await response.Content.ReadAsStringAsync();
                }).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error de comunicación con {API}, status code: {response.StatusCode.ToString()}");
                }

                _Response = JsonConvert.DeserializeObject(respuesta, _Response.GetType());

                Response = _Response;
                httpEvent = true;
            }
            catch (Exception ex)
            {
                Message = $"Error al consumir el método POST, mas detalle: {ex.Message}";
                httpEvent = false;
            }
        }

        public void PostAsync(String API, String URI, dynamic _Request, dynamic _Response)
        {
            String JsonRequest = JsonConvert.SerializeObject(_Request);

            HttpClient http = new HttpClient();

            StringContent oString = new StringContent(JsonRequest);
            oString.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = null;
            var respuesta = String.Empty;

            try
            {
                Task.Run(async () =>
                {
                    response = http.PostAsync((API + URI), oString).Result;
                    respuesta = await response.Content.ReadAsStringAsync();

                });

            }
            catch (Exception ex)
            {
                Message = $"Error al consumir API, mas detalle: {ex.Message}";
                httpEvent = false;
            }
        }

        public void Put(String API, String URI, dynamic _Request, dynamic _Response)
        {
            String JsonRequest = JsonConvert.SerializeObject(_Request);

            HttpClient http = new HttpClient();

            StringContent oString = new StringContent(JsonRequest);
            oString.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = null;
            var respuesta = String.Empty;

            try
            {
                Task.Run(async () =>
                {
                    response = http.PutAsync((API + URI), oString).Result;
                    respuesta = await response.Content.ReadAsStringAsync();

                }).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error de comunicación con {API}, status code: {response.StatusCode.ToString()}");
                }

                _Response = JsonConvert.DeserializeObject(respuesta, _Response.GetType());

                Response = _Response;
                httpEvent = true;
            }
            catch (Exception ex)
            {
                Message = $"Error al consumir el metodo PUT, mas detalle: {ex.Message}";
                httpEvent = false;
            }
        }

        public void Get(String API, String URI, dynamic _Response)
        {

            HttpClient http = new HttpClient();

            HttpResponseMessage response = null;
            var respuesta = String.Empty;

            try
            {
                Task.Run(async () =>
                {
                    response = http.GetAsync((API + URI)).Result;
                    respuesta = await response.Content.ReadAsStringAsync();

                }).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error de comunicación con {API}, status code: {response.StatusCode.ToString()}");
                }

                _Response = new JSON().DeserializeJSON(respuesta, _Response);


                Response = _Response;
                httpEvent = true;
            }
            catch (Exception ex)
            {
                Message = $"Error al consumir el metodo Get, mas detalles: {ex.Message}";
                httpEvent = false;
            }
        }

        public void GetAsync(String API, String URI, dynamic _Response)
        {

            HttpClient http = new HttpClient();

            HttpResponseMessage response = null;
            var respuesta = String.Empty;

            try
            {
                Task.Run(async () =>
                {
                    response = http.GetAsync((API + URI)).Result;
                });
            }
            catch (Exception ex)
            {
                Message = $"Comprobante no autorizado, mas detalles: {ex.Message}";
                httpEvent = false;
            }

            Response = _Response;

            httpEvent = true;
        }

    }
}
