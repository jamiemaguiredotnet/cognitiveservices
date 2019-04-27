using ContactAssistant.Models;
using ContactAssistant.StateManagement;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContactAssistant.Dialogs.Contacts
{
    public class AddContactDialogue : ComponentDialog
    {
        // Prompts ids
        private const string IdPrompt = "idPrompt";
        private const string NamePrompt = "namePrompt";
        private const string AgePrompt = "agePrompt";

        // Dialog IDs
        private const string AddContactDialogId = "AddContactDialogue";

        private StateBotAccessors StatePropertyAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddContactDialogue"/> class.
        /// </summary>
        /// <param name="botServices">Connected services used in processing.</param>
        /// <param name="statePropertyAccessor">The <see cref="StateBotAccessors"/> for storing properties at user-scope.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> that enables logging and tracing.</param>
        public AddContactDialogue(StateBotAccessors statePropertyAccessor, ILoggerFactory loggerFactory)
            : base(nameof(AddContactDialogue))
        {
            StatePropertyAccessor = statePropertyAccessor;

            // Add control flow dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                    InitializeStateStepAsync,
                    PromptForIdStepAsync,
                    PromptForNameStepAsync,
                    PromptForAgeStepAsync,
                    DialogueComplete
            };
            AddDialog(new WaterfallDialog(AddContactDialogId, waterfallSteps));
            AddDialog(new TextPrompt(IdPrompt));
            AddDialog(new TextPrompt(NamePrompt));
            AddDialog(new TextPrompt(AgePrompt));
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // initialise the contact
            Contact contact = new Contact();
            await StatePropertyAccessor.ContactAccessor.SetAsync(stepContext.Context, contact);

            await StatePropertyAccessor.ConversationState.SaveChangesAsync(stepContext.Context);

            return await stepContext.NextAsync(contact);
        }

        private async Task<DialogTurnResult> PromptForIdStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Contact contact = await StatePropertyAccessor.ContactAccessor.GetAsync(stepContext.Context, () => new Contact());

            if (contact.id == 0)
            {
                var opts = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Please supply me with a unique id for this contact:",
                    },
                };
                return await stepContext.PromptAsync(IdPrompt, opts);
            }
            else
            {
                return await stepContext.NextAsync(contact);
            }
        }

        private async Task<DialogTurnResult> PromptForNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // take the ID from the previous step
            Contact contact = await StatePropertyAccessor.ContactAccessor.GetAsync(stepContext.Context);

            contact.id = int.Parse(stepContext.Result.ToString());

            //save id property to state
            await StatePropertyAccessor.ContactAccessor.SetAsync(stepContext.Context, contact);

            if (string.IsNullOrEmpty(contact.name))
            {
                var opts = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "What is the contacts name?",
                    },
                };
                return await stepContext.PromptAsync(NamePrompt, opts);
            }
            else
            {
                return await stepContext.NextAsync(contact);
            }
        }

        private async Task<DialogTurnResult> PromptForAgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // take the Name from the previous step
            Contact contact = await StatePropertyAccessor.ContactAccessor.GetAsync(stepContext.Context);

            contact.name = stepContext.Result.ToString();

            // save name property to state
            await StatePropertyAccessor.ContactAccessor.SetAsync(stepContext.Context, contact);

            if (contact.age == 0)
            {
                var opts = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "What age is the contact? (25, 39 etc.)",
                    },
                };
                return await stepContext.PromptAsync(AgePrompt, opts);
            }
            else
            {
                return await stepContext.NextAsync();
            }
        }

        private async Task<DialogTurnResult> DialogueComplete(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Contact contact = await StatePropertyAccessor.ContactAccessor.GetAsync(stepContext.Context);
            ContactList contacts = await StatePropertyAccessor.ContactListAccessor.GetAsync(stepContext.Context);

            // take the Age from the previous step
            contact.age = int.Parse(stepContext.Result.ToString());

            if (contact.id > 0 && contact.age > 0 && !string.IsNullOrEmpty(contact.name))
            {
                await stepContext.Context.SendActivityAsync($"Successfully added contact with id of: { contact.id }, named: { contact.name} whos is { contact.age} years old.");

                contacts.Add(contact);
                // save list
                await StatePropertyAccessor.ContactListAccessor.SetAsync(stepContext.Context, contacts);

                return await stepContext.EndDialogAsync();
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(this.Id);
            }

        }

    }

}
