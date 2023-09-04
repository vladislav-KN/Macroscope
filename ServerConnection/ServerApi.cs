using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Xml;

namespace ServerConnection
{
    public class ServerApi
    {
        private XmlDocument _xmlDoc;
        private string? apiUrl;
        public string? ApiUrl
        {
            get
            {
                return apiUrl;
            }
            set
            {
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                {
                    apiUrl = value;
                }
                else
                {
                    throw new InvalidOperationException("Ссылка не корректна.");
                }
            }
        }
        public ServerApi(string? url)
        {
            ApiUrl = url;
            _xmlDoc = new XmlDocument();
        }

        public ServerApi()
        {
            _xmlDoc = new XmlDocument();
        }
        /// <summary>
        /// Получение новых данных с сервера
        /// </summary>
        public async void UpdateXML()
        {
            try
            {
                var client = new HttpClient();
                if (apiUrl is null)
                    return;
                var url = new Uri(apiUrl);
                var result = await client.GetAsync(url).Result.Content.ReadAsStringAsync();
 
                _xmlDoc.LoadXml(result);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("This program is expected to throwHttpRequestException on successful run." +
                                    "\n\nException Message :" + e.Message);
            }
            catch (XmlException e)
            {
                Console.WriteLine("This program is expected to throw XmlException on successful run." +
                                   "\n\nException Message :" + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public XmlDocument GetXmlDoc()
        {
            return _xmlDoc;
        }
        /// <summary>
        /// Получение информции из XML документа
        /// </summary>
        /// <param name="xmlDoc">документ</param>
        /// <returns></returns>
        public List<ApiData> GetData(XmlDocument xmlDoc)
        {
            List<ApiData> data = new List<ApiData>();
            var xml = new XmlDocument();
       
            foreach (XmlNode node in xmlDoc.GetElementsByTagName("ChannelInfo"))
            {
                if (node.Attributes is not null)
                {
                    var name = node.Attributes.GetNamedItem("Name");
                    var id = node.Attributes.GetNamedItem("Id");
                    if(name != null && id != null)
                    data.Add(new ApiData()
                    {
                        Name = name.Value,
                        Id = id.Value
                    });
                }
            }
            return data;
        }
    }
}