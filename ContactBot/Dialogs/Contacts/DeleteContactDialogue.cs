using ContactAssistant.Models;
using ContactAssistant.StateManagement;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContactAssistant.Dialogs.Contacts
{
    public class DeleteContactDialogue : ComponentDialog
    {
        // Dialog IDs
        private const string DeleteContactDialogId = "DeleteContactDialogue";

        // Prompts names
        private const string IdPrompt = "idPrompt";
        private const string confirmPrompt = "confirmPrompt";

        private StateBotAccessors StatePropertyAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewContactDialogue"/> class.
        /// </summary>
        /// <param name="botServices">Connected services used in processing.</param>
        /// <param name="statePropertyAccessor">The <see cref="StateBotAccessors"/> for storing properties at user-scope.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> that enables logging and tracing.</param>
        public DeleteContactDialogue(StateBotAccessors statePropertyAccessor, ILoggerFactory loggerFactory)
            : base(nameof(DeleteContactDialogue))
        {
            StatePropertyAccessor = statePropertyAccessor;

            // Add control flow dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                    PromptForContactDeletionAsync,
                    PromptConfirmDeletionAsync,
                    DialogueComplete
            };

            AddDialog(new WaterfallDialog(DeleteContactDialogId, waterfallSteps));
            AddDialog(new TextPrompt(IdPrompt));
            AddDialog(new ConfirmPrompt(confirmPrompt));
        }

        private async Task<DialogTurnResult> PromptForContactDeletionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ContactList contacts = await StatePropertyAccessor.ContactListAccessor.GetAsync(stepContext.Context);

            if (contacts.Count > 0)
            {
                await stepContext.Context.SendActivityAsync($"I'm fetching you a list of existing contacts");

                foreach (Contact contact in contacts)
                {
                    await stepContext.Context.SendActivityAsync($"Contact id: { contact.id }, name: { contact.name} , age: { contact.age} years old.");
                }
                var opts = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Please enter a contact id to delete:",
                    },
                };
                return await stepContext.PromptAsync(IdPrompt, opts);
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"No contacts have been created yet!");
                return await stepContext.EndDialogAsync();
            }
        }

        private async Task<DialogTurnResult> PromptConfirmDeletionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            int contactIdToDelete = int.Parse(stepContext.Result.ToString());

            ContactList contacts = await StatePropertyAccessor.ContactListAccessor.GetAsync(stepContext.Context);

            if (contactIdToDelete > 0)
            {
                Contact contact = contacts.Where(c => c.id == contactIdToDelete).First();

                await StatePropertyAccessor.ContactAccessor.SetAsync(stepContext.Context, contact);

                return await stepContext.PromptAsync(confirmPrompt,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Are you sure"),
                });
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"No contacts have been created yet!");
                return await stepContext.EndDialogAsync();
            }

        }

        private async Task<DialogTurnResult> DialogueComplete(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // get the contact from the prev step
            ContactList contacts = await StatePropertyAccessor.ContactListAccessor.GetAsync(stepContext.Context);
            bool deleteConfirmed = bool.Parse(stepContext.Result.ToString());

            if (deleteConfirmed)
            {
                // get the contact from state
                Contact contact = await StatePropertyAccessor.ContactAccessor.GetAsync(stepContext.Context);
                
                // remove the contact from the list
                if(contacts.RemoveAll(c => c.id == contact.id) > 0)
                {
                    // update the list
                    await StatePropertyAccessor.ContactListAccessor.SetAsync(stepContext.Context, contacts);
                    // re-initialise
                    await StatePropertyAccessor.ContactAccessor.SetAsync(stepContext.Context, new Contact());
                    // notify user
                    await stepContext.Context.SendActivityAsync($"Contact successfully deleted!");   
                }
                else
                {
                    await stepContext.Context.SendActivityAsync($"An error occurred whilst trying to remove the Contact");
                }
                // end the dialogue
                return await stepContext.EndDialogAsync();
            }
            else
            {
                // return to the start of this dialogue
                return await stepContext.ReplaceDialogAsync(DeleteContactDialogId);
            }
        }
    }
}
