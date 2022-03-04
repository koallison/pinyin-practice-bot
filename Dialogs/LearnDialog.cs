// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;


namespace Microsoft.BotBuilderSamples
{
    public class LearnDialog : ComponentDialog
    {
        // Define a "done" response for the company selection prompt.
        private const string DoneOption = "done";

        // Define value names for values tracked inside the dialogs.
        private const string UserInfo = "value-userInfo";

        private Dictionary<string, string> vocab = new Dictionary<string, string>();

        public LearnDialog()
            : base(nameof(LearnDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroductionStepAsync,
                LoopStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);

            vocab.Add("ma", "妈");
            vocab.Add("ma1", "妈");
            vocab.Add("ma2", "麻");
            vocab.Add("ma3", "马");
            vocab.Add("ma4", "骂");

            vocab.Add("si", "思");
            vocab.Add("si1", "思");
            vocab.Add("shi", "师");
            vocab.Add("shi1", "师");
            vocab.Add("lu", "路");
            vocab.Add("lu4", "路");
            vocab.Add("lv", "绿");
            vocab.Add("lv4", "绿");
        }

        private static async Task<DialogTurnResult> IntroductionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string message = "Type any pinyin to hear the pronunciation. Type \"quit\" at any time to stop.";

            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(message) };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> LoopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string pinyin = (string)stepContext.Result;

            string character = vocab.GetValueOrDefault(pinyin, "");
            if (pinyin == "quit")
            {
                return await stepContext.EndDialogAsync();
            }
            else if (character == "")
            {
                string message = "I didn't understand. Please try again.";
                await stepContext.Context.SendActivityAsync(message);
                return await stepContext.ReplaceDialogAsync(nameof(LearnDialog), null, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(SpeakChinese(pinyin, character), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(LearnDialog), null, cancellationToken);
            }
        }

        private Activity SpeakChinese(string textMessage, string speechMessage)
        {
            var activity = MessageFactory.Text(textMessage);
            string body = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>" +
                "<voice name='zh-CN-XiaohanNeural'>" + $"{speechMessage}" + "</voice></speak>";

            //activity.Speak = body;
            return activity;
        }
    }
}
