using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
//using System.Text.RegularExpressions;


namespace Onha.Kiet
{
    public class rapidfireart : GeneralSite
    {
        const string DOMAIN_HOST = @"http://rapidfireart.com/";
        public rapidfireart() : base(DOMAIN_HOST)
        {
        }

        #region Override methods
        protected override HtmlNode GetContentDiv(string htmlContent, bool cleanUp = false)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            var root = html.DocumentNode;
            var div = root.SelectSingleNode("//article[@itemtype='http://schema.org/Article']"); //class="story-body" //span[@style='font-size: medium;']");

            return div;
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetLinks(string htmlContent)
        {           
            return null;
        }

        protected override Book GetBookInformation(HtmlNode contentNode)
        {
            var book = new Book();

            var badChars = new char[] { '\r', '\n', ' '};
            var title = contentNode.SelectSingleNode("//h1[@class='entry-title']");
            var author = contentNode.SelectSingleNode("//div[@class='saboxplugin-authorname']");

            book.Title = "rapidfire art";
            book.Creator = "rapidfire art";
            book.Copyright = "rapidfire art";
            book.Publisher = "rapidfire art";

            if (title != null)
            {
                book.Title = title.InnerText.Trim(badChars);
            }

            if (author != null)
            {               
                book.Creator = author.InnerText.Trim(badChars);
            }

            return book;
        }

        protected override List<KeyValuePair<string, byte[]>> FixImages(HtmlNode div)
        {
            var imgNodes = div.Descendants("img");// .SelectNodes("//img");
            var images = new List<KeyValuePair<string, byte[]>>();

            foreach (var node in imgNodes)
            {
                var imagePath = node.GetAttributeValue("data-original", "");
                if (string.IsNullOrEmpty(imagePath))
                    imagePath = node.GetAttributeValue("src", "");

                var imageFile = System.IO.Path.GetFileName(imagePath);

                if (!FileNameSanitizer.IsBadName(imageFile))
                {
                    var imageBytesTask = webber.DownloadFile(imagePath);
                    byte[] imageBytes = null;

                    try
                    {
                        imageBytes = imageBytesTask.Result;

                        if (imageBytesTask.Status != System.Threading.Tasks.TaskStatus.Faulted)
                        {
                            images.Add(new KeyValuePair<string, byte[]>(imageFile, imageBytes));
                        }
                        node.SetAttributeValue("src", imageFile); // modify the name in source
                    }
                    catch (System.AggregateException ex)
                    {
                        // node.RemoveChild(node);
                    }
                    finally
                    {

                    }


                }
            }

            return images;
        }
        #endregion

    }
}