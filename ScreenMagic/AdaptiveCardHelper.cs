using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class AdaptiveCardHelper
    {
        public static string GetCard(string launchUri)
        {

            AdaptiveCard card = new AdaptiveCard();
            card.BackgroundImage = new Uri("https://demoicons.blob.core.windows.net/blobcontainer/bg.jpg");
            AdaptiveTextBlock title = new AdaptiveTextBlock
            {
                Text = "Hello World",
                Size = AdaptiveTextSize.Medium,
                Wrap = true
            };

            AdaptiveColumnSet columnSet = new AdaptiveColumnSet();

            AdaptiveColumn photoColumn = new AdaptiveColumn
            {
                Width = "auto"
            };
            AdaptiveImage image = new AdaptiveImage
            {
                Url = new Uri("https://pbs.twimg.com/profile_images/587911661526327296/ZpWZRPcp_400x400.jpg"),
                Size = AdaptiveImageSize.Small,
                Style = AdaptiveImageStyle.Person
            };
            photoColumn.Items.Add(image);

            AdaptiveTextBlock name = new AdaptiveTextBlock
            {
                Text = "Gerald Haslhofer",
                Weight = AdaptiveTextWeight.Bolder,
                Wrap = true
            };

            AdaptiveTextBlock date = new AdaptiveTextBlock
            {
                Text = DateTime.Now.ToShortDateString(),
                IsSubtle = true,
                Spacing = AdaptiveSpacing.None,
                Wrap = true
            };

            AdaptiveColumn authorColumn = new AdaptiveColumn
            {
                Width = "stretch"
            };
            authorColumn.Items.Add(name);
            authorColumn.Items.Add(date);

            columnSet.Columns.Add(photoColumn);
            columnSet.Columns.Add(authorColumn);

            AdaptiveTextBlock body = new AdaptiveTextBlock
            {
                Text = $"Let's say this is a very long text. Let's say this is a very long text. Let's say this is a very long text Let's say this is a very long text Let's say this is a very long text Let's say this is a very long text Let's say this is a very long text",
                Wrap = true
            };

            AdaptiveOpenUrlAction action = new AdaptiveOpenUrlAction
            {
                Url = new Uri(launchUri),
                Title = "View"
            };

            card.Body.Add(title);
            card.Body.Add(columnSet);
            card.Body.Add(body);
            card.Actions.Add(action);

            string json = card.ToJson();
            return json;
        }
    }
}
