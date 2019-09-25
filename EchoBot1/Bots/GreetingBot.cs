using EchoBot1.Models;
using EchoBot1.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Bots
{
    public class GreetingBot : ActivityHandler
    {
        #region Variables
        private readonly BotStateService _botstateservice;
        #endregion

        public GreetingBot(BotStateService botStateService)
        {
            _botstateservice = botStateService ?? throw new System.ArgumentNullException(nameof(BotStateService));
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if ( member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botstateservice.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
            ConversationData conversationData = await _botstateservice.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("Hi {0}. How can I help you today?", userProfile.Name)), cancellationToken);

            }
            else
            {
                if (conversationData.PromptedUserForName)
                {
                    // Set the name to what the user provided
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    // Acknowledge that we got their name
                    await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("Thanks {0}. How Can I help you today?", userProfile.Name)), cancellationToken);

                    // Reset the flag to allow the bot to go though the cycle again
                    conversationData.PromptedUserForName= false;
                }
                else
                {
                    // Prompt the user for their name
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"), cancellationToken);

                    // Set the flag to true, so we don't prompt in the next turn
                    conversationData.PromptedUserForName = true;
                }

                // Save any state changes that might have occured during the turn
                await _botstateservice.UserProfileAccessor.SetAsync(turnContext, userProfile);
                await _botstateservice.ConversationDataAccessor.SetAsync(turnContext, conversationData);

                await _botstateservice.UserState.SaveChangesAsync(turnContext);
                await _botstateservice.ConversationState.SaveChangesAsync(turnContext);
            }
        }
    }
}
