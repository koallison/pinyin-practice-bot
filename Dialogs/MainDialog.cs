// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : ComponentDialog
    {
        private readonly UserState _userState;
        private string _mode;

        public MainDialog(UserState userState)
            : base(nameof(MainDialog))
        {
            _userState = userState;

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new LearnDialog());
            AddDialog(new QuizDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string message = "What do you want to do today?";
            var options = new List<string>()
            {
                "learn",
                "quiz"
            };

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(message),
                RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                Choices = ChoiceFactory.ToChoices(options),
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = (FoundChoice)stepContext.Result;
            _mode = (string)choice.Value;

            if (_mode == "learn")
            {
                string message = "Let's learn pinyin.";
                await stepContext.Context.SendActivityAsync(message);
                return await stepContext.BeginDialogAsync(nameof(LearnDialog), null, cancellationToken);
            }
            else if (_mode == "quiz")
            {

                string message = "Let's start the quiz.";
                await stepContext.Context.SendActivityAsync(message);
                return await stepContext.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }
    }
}
