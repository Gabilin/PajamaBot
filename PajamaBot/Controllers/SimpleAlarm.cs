using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace PajamaBot.Controllers
{
    [LuisModel("a9cf7aa4-b26b-4f94-bb20-09e1c8ac621e", "1bb72a78c8604e22aabcfe2aafeb725d")]
    [Serializable]
    public class SimpleAlarmDialog : LuisDialog<object>
    {
        private readonly Dictionary<string, string> projects = new Dictionary<string, string>();
        public const string DefaultProjectName = "Project Name";
        public bool TryFindAlarm(LuisResult result, out string tfsItertaion)
        {
            tfsItertaion = string.Empty;
            string what;
            EntityRecommendation projectName;
            if (result.TryFindEntity(Entity_Project_Name, out projectName))
            {
                what = projectName.Entity;
            }
            else
            {
                what = DefaultProjectName;
            }
            return this.projects.TryGetValue(what, out tfsItertaion);
        }
        public const string Entity_Project_Name = "Project Details::Project Name";
        public const string Entity_Alarm_Start_Time = "builtin.alarm.start_time";
        public const string Entity_TFS_iteration = "Project Details::TFS iteration";
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = "Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //[LuisIntent("builtin.intent.alarm.delete_alarm")]
        //public async Task DeleteAlarm(IDialogContext context, LuisResult result)
        //{
        //    Alarm alarm;
        //    if (TryFindAlarm(result, out alarm))
        //    {
        //        this.alarmByWhat.Remove(alarm.What);
        //        await context.PostAsync("alarm "+alarm+" deleted");
        //    }
        //    else
        //    {
        //        await context.PostAsync("did not find alarm");
        //    }
        //    context.Wait(MessageReceived);
        //}
        //[LuisIntent("builtin.intent.alarm.find_alarm")]
        //public async Task FindAlarm(IDialogContext context, LuisResult result)
        //{
        //    Alarm alarm;
        //    if (TryFindAlarm(result, out alarm))
        //    {
        //        await context.PostAsync("found alarm "+alarm);
        //    }
        //    else
        //    {
        //        await context.PostAsync("did not find alarm");
        //    }
        //    context.Wait(MessageReceived);
        //}

        [LuisIntent("intent.project.register_project")]
        public async Task RegisterProject(IDialogContext context, LuisResult result)
        {
            EntityRecommendation projectName;
            if (! result.TryFindEntity(Entity_Project_Name, out projectName))
            {
                projectName = new EntityRecommendation(type: Entity_Project_Name) { Entity = DefaultProjectName };
            }
            EntityRecommendation iteration;
            if (! result.TryFindEntity(Entity_TFS_iteration, out iteration))
            {
                iteration = new EntityRecommendation(type: Entity_TFS_iteration) { Entity = string.Empty };
            }

            //if(!string.IsNullOrEmpty(iteration.Entity))
            //{
            //    Chain.Return("I was not able to detect the iteration. Can you please provide it or type 'Cancel'").PostToUser();
            //    Chain.PostToChain().Select(m => m.Text).Switch(
            //        Chain.Case(new Regex("^cancel"), (context2, text) => Chain.Return("Operation canceled").PostToUser())).PostToUser();

            //   // await context.PostAsync("I was not able to detect the iteration. Can you please provide it or type 'Cancel'"); 
            //}
            
            if (!string.IsNullOrEmpty(projectName.Entity) && !string.IsNullOrEmpty(iteration.Entity))
            {
                this.projects[projectName.Entity] = iteration.Entity;
                await context.PostAsync("I have successfuly registered a project");
            }
            else
            {
                await context.PostAsync("could not find time for alarm");
            }
            context.Wait(MessageReceived);
        }
        //[LuisIntent("builtin.intent.alarm.snooze")]
        //public async Task AlarmSnooze(IDialogContext context, LuisResult result)
        //{
        //    Alarm alarm;
        //    if (TryFindAlarm(result, out alarm))
        //    {
        //        alarm.When = alarm.When.Add(TimeSpan.FromMinutes(7));
        //        await context.PostAsync("alarm "+alarm+" snoozed!");
        //    }
        //    else
        //    {
        //        await context.PostAsync("did not find alarm");
        //    }
        //    context.Wait(MessageReceived);
        //}
        //[LuisIntent("builtin.intent.alarm.time_remaining")]
        //public async Task TimeRemaining(IDialogContext context, LuisResult result)
        //{
        //    Alarm alarm;
        //    if (TryFindAlarm(result, out alarm))
        //    {
        //        var now = DateTime.UtcNow;
        //        if (alarm.When > now)
        //        {
        //            var remaining = alarm.When.Subtract(DateTime.UtcNow);
        //            await context.PostAsync("There is "+remaining+" remaining for alarm "+alarm);
        //        }
        //        else
        //        {
        //            await context.PostAsync("The alarm "+alarm+" expired already.");
        //        }
        //    }
        //    else
        //    {
        //        await context.PostAsync("did not find alarm");
        //    }
        //    context.Wait(MessageReceived);
        //}
        //private Alarm turnOff;
        //[LuisIntent("builtin.intent.alarm.turn_off_alarm")]
        //public async Task TurnOffAlarm(IDialogContext context, LuisResult result)
        //{
        //    if (TryFindAlarm(result, out this.turnOff))
        //    {
        //        PromptDialog.Confirm(context, AfterConfirming_TurnOffAlarm, "Are you sure?");
        //    }
        //    else
        //    {
        //        await context.PostAsync("did not find alarm");
        //        context.Wait(MessageReceived);
        //    }
        //}
        //public async Task AfterConfirming_TurnOffAlarm(IDialogContext context, IAwaitable<bool> confirmation)
        //{
        //    if (await confirmation)
        //    {
        //        this.alarmByWhat.Remove(this.turnOff.What);
        //        await context.PostAsync("Ok, alarm "+this.turnOff+" disabled.");
        //    }
        //    else
        //    {
        //        await context.PostAsync("Ok! We haven't modified your alarms!");
        //    }
        //    context.Wait(MessageReceived);
        //}
        [LuisIntent("builtin.intent.alarm.alarm_other")]
        public async Task AlarmOther(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("what ?");
            context.Wait(MessageReceived);
        }
        public SimpleAlarmDialog(ILuisService service = null)
            : base(service)
        {
        }
        [Serializable]
        public sealed class Alarm : IEquatable<Alarm>
        {
            public DateTime When { get; set; }
            public string What { get; set; }
            public override string ToString()
            {
                return "["+this.What+" at "+this.When+"]";
            }
            public bool Equals(Alarm other)
            {
                return other != null
                    && this.When == other.When
                    && this.What == other.What;
            }
            public override bool Equals(object other)
            {
                return Equals(other as Alarm);
            }
            public override int GetHashCode()
            {
                return this.What.GetHashCode();
            }
        }
    }
    }
