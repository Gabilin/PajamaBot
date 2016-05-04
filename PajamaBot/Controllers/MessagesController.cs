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
using Microsoft.Bot.Builder.Dialogs;
using PajamaBot.Controllers;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;
using System.Text.RegularExpressions;

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
                // return our reply to the user
                
                return await Conversation.SendAsync(message, MakeRootDialog);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private void Test()
        {
            //if (message.Type == "Message")
            //{

            //    string reply;
            //    var counter = message.GetBotPerUserInConversationData<int>("counter");
            //    if (counter == 0)
            //    {
            //        reply = "Hey there! How Can I help you?\\n1) Create a project\\n2)Change Project Settings\\n3)Show project info & stats";
            //        counter = 1;
            //    }
            //    else if (counter == 1)
            //    {
            //        if (message.Text.StartsWith("1") || message.Text.StartsWith("first"))
            //        {
            //            reply = "Okey, let us create a new project.In you next reply, specify the TFS iteration, project team and email i can use to notify the whole team.\\Example:" + @"Products\SC8.1\Update-3\Sustained Engineering\SC8.1U3-Pool for bugfixing" + "|" + "AVA;SMAR;DS" + "|" + "DepartmentPRodcutSEteam@sitecore.net";
            //            counter = 2;
            //        }
            //        else
            //        {
            //            reply = "Not implemented";
            //            counter = 0;
            //        }
            //    }
            //    else if (counter == 2)
            //    {
            //        //Here goes the parsing of the input parameters and confirmation
            //        reply = "Not Implemnted";
            //        counter = 0;
            //    }
            //    else
            //    {
            //        reply = "I am not so smart, sorry!";
            //        counter = 0;
            //    }

            //    var replyMessage = message.CreateReplyMessage(reply);

            //    replyMessage.SetBotPerUserInConversationData("counter", counter);

            //    return replyMessage;


            //    //// fetch our state associated with a user in a conversation. If we don't have state, we get default(T)
            //    //var counter = message.GetBotPerUserInConversationData<int>("counter");

            //    //// create a reply message   
            //    //Message replyMessage = message.CreateReplyMessage($"{++counter} You said:{message.Text}");

            //    //// save our new counter by adding it to the outgoing message
            //    //replyMessage.SetBotPerUserInConversationData("counter", counter);

            //    //// return our reply to the user
            //    //return replyMessage;

            //}
            //else
            //{
            //    return HandleSystemMessage(message);
            //}
        }

        internal static IDialog<string> MakeRootDialog()
        {
            IDialog<string> joke = Chain
                .PostToChain()
                .Select(m => m.Text)
                .Switch
                (
                    Chain.Case
                    (
                        new Regex("^chicken"),
                        (context, text) =>
                            Chain
                            .Return("why did the chicken cross the road?")
                            .PostToUser()
                            .WaitToBot()
                            .Select(ignoreUser => "to get to the other side")
                    ),
                    Chain.Default<string, IDialog<string>>(
                        (context, text) =>
                            Chain
                            .Return("why don't you like chicken jokes?")
                    )
                )
                .Unwrap()
                .PostToUser().
                Loop();
            return joke;
            //return Chain.From(() => FormDialog.FromForm(PjMenu.BuildForm));
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

    public enum PjMOptions
    {
        CreateProject, ProjectActions, ViewProjectInformation, ProjectStatistic
    };
    public enum ProjectOptions { SixInch, FootLong };
    //public enum BreadOptions { NineGrainWheat, NineGrainHoneyOat, Italian, ItalianHerbsAndCheese, Flatbread };
    //public enum CheeseOptions { American, MontereyCheddar, Pepperjack };
    //public enum ToppingOptions
    //{
    //    Avocado, BananaPeppers, Cucumbers, GreenBellPeppers, Jalapenos,
    //    Lettuce, Olives, Pickles, RedOnion, Spinach, Tomatoes
    //};
    //public enum SauceOptions
    //{
    //    ChipotleSouthwest, HoneyMustard, LightMayonnaise, RegularMayonnaise,
    //    Mustard, Oil, Pepper, Ranch, SweetOnion, Vinegar
    //};

    [Serializable]
    class PjMenu
    {
        public PjMOptions? Operation;
        public ProjectOptions? Action;        
        
        public static IForm<PjMenu> BuildForm()
        {
            return new FormBuilder<PjMenu>()
                    .Message("Welcome to the Project Manager Bot!")
                    .Build();
        }
    };

    

    [Serializable]
    public class PjMActionsDialog : IDialog<object>
    {
        private int count = 1;
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!");
            }
            else
            {
                await context.PostAsync(string.Format("{0}: You said {1}", this.count++, message.Text));
                context.Wait(MessageReceivedAsync);
            }
        }
        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}