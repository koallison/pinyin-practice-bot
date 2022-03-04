// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;


namespace Microsoft.BotBuilderSamples
{
    public class QuizDialog : ComponentDialog
    {
        private const string CorrectAnswer = "value-correctAnswer";

        private Dictionary<string, string> vocab = new Dictionary<string, string>();

        public QuizDialog()
            : base(nameof(QuizDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroductionStepAsync,
                LoopStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);

            vocab.Add("ma1", "妈");
            vocab.Add("ma2", "麻");
            vocab.Add("ma3", "马");
            vocab.Add("ma4", "骂");

            vocab.Add("si1", "思");
            vocab.Add("shi1", "师");
            vocab.Add("lu4", "路");
            vocab.Add("lv4", "绿");
        }

        private async Task<DialogTurnResult> IntroductionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Random rand = new Random();
            string correct_answer = vocab.ElementAt(rand.Next(0, vocab.Count)).Key;
            string character = vocab.GetValueOrDefault(correct_answer); 
            stepContext.Values[CorrectAnswer] = correct_answer;

            string message = "What is the pinyin for this word? Type \"quit\" at any time to stop.";

            var promptOptions = new PromptOptions { Prompt = SpeakChinese(message, character) };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> LoopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string user_answer = (string)stepContext.Result;
            string correct_answer = (string)stepContext.Values[CorrectAnswer];

            if (user_answer == "quit")
            {
                return await stepContext.EndDialogAsync();
            }
            else if (user_answer == correct_answer)
            {
                string message = "That's correct!";
                await stepContext.Context.SendActivityAsync(message);
                return await stepContext.ReplaceDialogAsync(nameof(QuizDialog), null, cancellationToken);
            }
            else
            {
                string message = $"Incorrect. The correct answer is {correct_answer}. Try again.";
                await stepContext.Context.SendActivityAsync(message);
                return await stepContext.ReplaceDialogAsync(nameof(QuizDialog), null, cancellationToken);
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
