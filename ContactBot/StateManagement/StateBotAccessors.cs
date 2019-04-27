using ContactAssistant.Models;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactAssistant.StateManagement
{
    public class StateBotAccessors
    {
        public StateBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        public ConversationState ConversationState { get; }
        public UserState UserState { get; }

        public static string ConversationDataName { get; } = "ConversationData";
        public static string ContactAccessorName { get; } = "ContactAccessor";
        public static string ContactListAccessorName { get; } = "ContactListAccessor";

        public IStatePropertyAccessor<Contact> ContactAccessor { get; set; }
        public IStatePropertyAccessor<ContactList> ContactListAccessor { get; set; }
    }
}
