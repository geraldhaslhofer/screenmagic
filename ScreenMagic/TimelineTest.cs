using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;

namespace ScreenMagic
{
    class TimelineTest
    {
        private static UserActivitySession _currentActivity;
        private static long _id = 11000;
        public async static void Setup()
        {
            UserActivityChannel channel = UserActivityChannel.GetDefault();
            UserActivity userActivity = await channel.GetOrCreateUserActivityAsync("MainPage");
            _currentActivity = userActivity.CreateSession();

        }
        public static async Task GenerateActivityAsync()
        {
            try
            {
                _id++;

                //Get the default UserActivityChannel and query it for our UserActivity. If the activity doesn't exist, one is created.
                UserActivityChannel channel = UserActivityChannel.GetDefault();
                UserActivity userActivity = await channel.GetOrCreateUserActivityAsync("MainPage" + _id.ToString());

                //Populate required properties

                userActivity.VisualElements.DisplayText = "Hello Activities + " + _id.ToString();

                userActivity.ActivationUri = new Uri("timelinetest://page2?action=edit");

                //Save
                await userActivity.SaveAsync(); //save the new metadata

                //Dispose of any current UserActivitySession, and create a new one.
                _currentActivity?.Dispose();
                _currentActivity = userActivity.CreateSession();
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
