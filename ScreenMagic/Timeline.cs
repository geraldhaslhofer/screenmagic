using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Shell;

namespace ScreenMagic
{
    class Timeline
    {
        private static UserActivitySession _currentActivity;
        private static string PROTOCOL = "screenmagic://";
        public static async Task GenerateActivityAsync(long id, string displayText, string description)
        {
            //Get the default UserActivityChannel and query it for our UserActivity. If the activity doesn't exist, one is created.
            UserActivityChannel channel = UserActivityChannel.GetDefault();
            UserActivity userActivity = await channel.GetOrCreateUserActivityAsync("MainPage");

            

            //Populate required properties
            userActivity.VisualElements.DisplayText = displayText;
            userActivity.VisualElements.AttributionDisplayText = "AttributionDisplayText";
            userActivity.VisualElements.Description = description;

            string json = AdaptiveCardHelper.GetCard(CreateUriFromId(id));

            //var card = new AdaptiveCard();
            //card.Title = "Title";
            
            //card.FallbackText = "Fallbacktext";
           
            //card.BackgroundImage = new Uri("https://demoicons.blob.core.windows.net/blobcontainer/bg.jpg");
            userActivity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(json);
            
            userActivity.ActivationUri = new Uri(CreateUriFromId(id));
     
            //Save
            await userActivity.SaveAsync(); //save the new metadata

            

            //Dispose of any current UserActivitySession, and create a new one.
            _currentActivity?.Dispose();
            _currentActivity = userActivity.CreateSession();
        }

        private static string CreateUriFromId(long id)
        {
            return (PROTOCOL + id.ToString().Trim());
        }

        public static long GetIdFromUri(string uri)
        {
            string uriSanitized = uri.ToLower();
            if (uri.StartsWith(PROTOCOL))
            {
                uriSanitized = uriSanitized.TrimEnd('/');
                string candidate = uriSanitized.Substring(PROTOCOL.Length);
                long result;
                if (long.TryParse(candidate, out result))
                {
                    return result;
                }
            }
            return 0;
        }



    }
}
