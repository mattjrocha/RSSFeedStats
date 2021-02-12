using System;
using System.ServiceModel.Syndication;
using System.Xml;
using NUnit.Framework;

namespace RSSFeedStats.RSS
{
    /*
     * FeedStats class takes a sitename and siteurl and returns the inactivity from that date.
    */
    [TestFixture]
    public class FeedStats : IFeedStats
    {
        public string SiteName;
        public string SiteURL;
        public string SiteInactivity;

        /* Constructor Method
         */
        public FeedStats(string siteName, string siteURL)
        {
            SiteName = siteName;
            SiteURL = siteURL;
            SiteInactivity = SetInactivity();
        }
        /*
         *For Testing Purposes only
         */
        [SetUp]
        public void SetUp()
        {
            SiteName = "CBS Boston";
            SiteURL = "https://boston.cbslocal.com/feed/";
        }

        /* SET INACTIVITY
         * Uses get Inactivity to set the objects 
         * days since last active feed.
         */
        [Test]
        public string SetInactivity()
        {
            SyndicationFeed RSSFeed;
            try
            {
                XmlReader xmlReader = XmlReader.Create(SiteURL);
                try
                {
                    RSSFeed = SyndicationFeed.Load(xmlReader);
                }
                catch (Exception ex)
                {
                    string exception = ex.Message.ToString();
                    return "N/A";//XML error
                }
                xmlReader.Close();
                if (RSSFeed != null)
                {
                    return GetInactivity(RSSFeed).ToString();
                }
                else
                {
                    return "N/A";
                }
            }
            catch (Exception ex)
            {
                string exception = ex.Message.ToString();
                return "N/A";//error 404
            }
        }

        /* GET INACTIVITY
         * Subtracts todays date from the feeds publishing date
         * Finds the newest published article for that feed
         * Returns how many days from current date.
         */
        private int GetInactivity(SyndicationFeed feed)
        {
            int lowInactivity = 999999;//certainly before internet
            foreach (SyndicationItem item in feed.Items)
            {
                DateTime curDate = DateTime.UtcNow;
                DateTime pubDate = item.PublishDate.UtcDateTime;
                DateTime defDate = default(DateTime);//default date
                if (!pubDate.Date.Equals(defDate.Date))//skip anything with default date.
                {
                    TimeSpan subDate = curDate.Subtract(pubDate.Date);
                    bool success = int.TryParse(subDate.Days.ToString(), out int itemInactivity);
                    if (itemInactivity < lowInactivity)//find newest active story
                    {
                        lowInactivity = itemInactivity;
                    }
                }

            }
            return lowInactivity;//days last feed
        }

    }
}
