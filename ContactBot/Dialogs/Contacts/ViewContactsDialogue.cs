using ContactAssistant.Models;
using ContactAssistant.StateManagement;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ContactAssistant.Dialogs.Contacts
{
    public class ViewContactsDialogue : ComponentDialog
    {
        // Dialog IDs
        private const string ViewContactDialogId = "ViewContactsDialogue";

        private StateBotAccessors StatePropertyAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewContactDialogue"/> class.
        /// </summary>
        /// <param name="botServices">Connected services used in processing.</param>
        /// <param name="statePropertyAccessor">The <see cref="StateBotAccessors"/> for storing properties at user-scope.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> that enables logging and tracing.</param>
        public ViewContactsDialogue(StateBotAccessors statePropertyAccessor, ILoggerFactory loggerFactory)
            : base(nameof(ViewContactsDialogue))
        {
            StatePropertyAccessor = statePropertyAccessor;

            // Add control flow dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                    DisplayContactsAsync,
                    DialogueComplete
            };
            AddDialog(new WaterfallDialog(ViewContactDialogId, waterfallSteps));
        }

        private async Task<DialogTurnResult> DisplayContactsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ContactList contacts = await StatePropertyAccessor.ContactListAccessor.GetAsync(stepContext.Context, () => new ContactList());

            if(contacts.Count > 0)
            {
                foreach (Contact contact in contacts)
                {
                    // display each contact
                    await stepContext.Context.SendActivityAsync($"Contact id: " + contact.id + ", Name:" + contact.name + ", Age:" + contact.age);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"No contacts exist!");
            }
            
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> DialogueComplete(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // we just end this dialogue and return the the main bot code
            return await stepContext.EndDialogAsync();
        }
    }
}
