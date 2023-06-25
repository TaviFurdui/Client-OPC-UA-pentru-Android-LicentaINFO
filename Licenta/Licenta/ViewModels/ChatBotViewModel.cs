using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using MvvmHelpers;
using Xamarin.Forms;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Opc.UaFx.Client;
using Opc.UaFx;
using Licenta.Models;

namespace Licenta.ViewModels
{
    class ChatBotViewModel : BaseViewModel
    {
        public string Query { get; set; }
        public Command SendQuery { get; set; }
        public ObservableRangeCollection<MessageModel> Messages { get; set; } = new ObservableRangeCollection<MessageModel>();
        public ChatBotViewModel()
        {
            SendQuery = new Command(OnSendQuery);
        }
        private async void OnSendQuery()
        {
            await GetAPIResponse(Query);
        }

        private static readonly HttpClient client = new HttpClient();
        public async Task<string> GetAPIResponse(string query)
        {
            string apiUrl = $"https://2e4a-86-105-64-37.ngrok-free.app?query={query}";
            Messages.Add(new MessageModel(query, MessageSender.User, "White"));
            Query = "";
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    JObject data = JObject.Parse(responseBody);
                    string nodeName = data["NodeName"].ToString();
                    string value = data["Value"].ToString();
                    string intent = data["Intent"].ToString();

                    if (intent == "Write")
                    {
                        try
                        {
                            WriteValue(nodeName, value);
                            Messages.Add(new MessageModel("Node name is " + nodeName + " and the value is " + value + ". I successfully wrote the value to the node.", MessageSender.Bot, "#7DC1E7"));
                        }
                        catch 
                        {
                            Messages.Add(new MessageModel("There was an error when searching for the node or when interpreting the value.", MessageSender.Bot, "#7DC1E7"));
                        }
                    }
                    else
                    {
                        try
                        {
                            string nodeValue = ReadValue(nodeName);
                            Messages.Add(new MessageModel("The value of " + nodeName + " is " + nodeValue + ".", MessageSender.Bot, "#7DC1E7"));
                        }
                        catch
                        {
                            Messages.Add(new MessageModel("There was an error when searching for the node or when interpreting the value.", MessageSender.Bot, "#7DC1E7"));
                        }
                    }
                    return responseBody;
                }
                else
                {
                    Console.WriteLine("API request failed with status code: " + response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while calling the API: " + ex.Message);
                return null;
            }
        }
        private void WriteValue(string nodename, string value)
        {
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                var node = client.BrowseNode("ns=3;s=" + nodename);
                if (node is OpcVariableNodeInfo variableNode)
                {
                    OpcNodeId dataTypeId = variableNode.DataTypeId;
                    OpcDataTypeInfo dataType = client.GetDataTypeSystem().GetType(dataTypeId);
                    Console.WriteLine(dataType.Name.ToString().ToLower());

                    switch (dataType.Name.ToString().ToLower())
                    {
                        case "boolean":
                            client.WriteNode("ns=3;s=" + nodename, Convert.ToBoolean(value));
                            break;
                        case "double":
                            client.WriteNode("ns=3;s=" + nodename, Convert.ToDouble(value));
                            break;
                    }
                }
            }
        }
        private string ReadValue(string nodename)
        {
            using (var client = new OpcClient(ServerModel.ServerIP))
            {
                client.Connect();
                return client.ReadNode("ns=3;s=" + nodename).ToString();
            }
        }
    }
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserMessageTemplate { get; set; }
        public DataTemplate BotMessageTemplate { get; set; }

        public DataTemplate SelectedTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return SelectedTemplate;
        }
    }
}
