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
        public static string GetCard(string titleText, string bodyText, string launchUri, string backgroundImagePath)
        {

            AdaptiveCard card = new AdaptiveCard();
            //card.BackgroundImage = new Uri(backgroundImagePath);
            AdaptiveTextBlock title = new AdaptiveTextBlock
            {
                Text = titleText,
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
                //Url = new Uri("https://pbs.twimg.com/profile_images/587911661526327296/ZpWZRPcp_400x400.jpg"),
                Url = new Uri(backgroundImagePath),
                Size = AdaptiveImageSize.Auto,
                Style = AdaptiveImageStyle.Default
            };
            photoColumn.Items.Add(image);

            AdaptiveTextBlock name = new AdaptiveTextBlock
            {
                Text = bodyText,
                Weight = AdaptiveTextWeight.Bolder,
                Wrap = true
            };

            //AdaptiveTextBlock date = new AdaptiveTextBlock
            //{
            //    Text = DateTime.Now.ToShortDateString(),
            //    IsSubtle = true,
            //    Spacing = AdaptiveSpacing.None,
            //    Wrap = true
            //};

            AdaptiveColumn authorColumn = new AdaptiveColumn
            {
                Width = "stretch"
            };
            authorColumn.Items.Add(name);
            //authorColumn.Items.Add(date);

            columnSet.Columns.Add(photoColumn);
            columnSet.Columns.Add(authorColumn);

            //AdaptiveTextBlock body = new AdaptiveTextBlock
            //{
            //    Text = bodyText,
            //    Wrap = true
            //};

            AdaptiveOpenUrlAction action = new AdaptiveOpenUrlAction
            {
                Url = new Uri(launchUri),
                Title = "View"
            };

            card.Body.Add(title);
            card.Body.Add(columnSet);
            //card.Body.Add(body);
            card.Actions.Add(action);

            string json = card.ToJson();
            return json;
        }
    }
}
