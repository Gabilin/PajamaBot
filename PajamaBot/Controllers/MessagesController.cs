using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace PajamaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {

                string reply;
                var counter = message.GetBotPerUserInConversationData<int>("counter");
                if (counter == 0 )
                {
                    reply = "Hey there! How Can I help you?\\n1) Create a project\\n2)Change Project Settings\\n3)Show project info & stats";
                    counter = 1;
                }
                else if (counter == 1)
                {
                    if (message.Text.StartsWith("1") || message.Text.StartsWith("first"))
                    {
                        reply = "Okey, let us create a new project.In you next reply, specify the TFS iteration, project team and email i can use to notify the whole team.\\Example:" + @"Products\SC8.1\Update-3\Sustained Engineering\SC8.1U3-Pool for bugfixing" + "|" + "AVA;SMAR;DS" + "|" + "DepartmentPRodcutSEteam@sitecore.net";
                        counter = 2;
                    }
                    else
                    {
                        reply = "Not implemented";
                        counter = 0;
                    }
                }
                else if (counter == 2)
                {
                    //Here goes the parsing of the input parameters and confirmation
                    reply = "Not Implemnted";
                    counter = 0;
                }
                else
                {
                    reply = "I am not so smart, sorry!";
                    counter = 0;
                }

                var replyMessage = message.CreateReplyMessage(reply);

                replyMessage.SetBotPerUserInConversationData("counter", counter);

                return replyMessage;
                

                //// fetch our state associated with a user in a conversation. If we don't have state, we get default(T)
                //var counter = message.GetBotPerUserInConversationData<int>("counter");

                //// create a reply message   
                //Message replyMessage = message.CreateReplyMessage($"{++counter} You said:{message.Text}");

                //// save our new counter by adding it to the outgoing message
                //replyMessage.SetBotPerUserInConversationData("counter", counter);

                //// return our reply to the user
                //return replyMessage;

            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                var replyMessage = message.CreateReplyMessage("User is removed from the COnversation");

                replyMessage.SetBotPerUserInConversationData("counter", 0);

                return replyMessage;
            }
            else if (message.Type == "BotAddedToConversation")
            {
                return message.CreateReplyMessage("Now you all wear pajamas");
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}