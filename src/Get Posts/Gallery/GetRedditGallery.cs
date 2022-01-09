namespace Dottik.MemeDownloader
{
    internal struct FormattedLinks
    {
        /// <summary>
        /// The Links in order 0 -> 1 -> 2 -> 3 -> 4...
        /// </summary>
        public List<string> Links = new();
        /// <summary>
        /// The Extensions of the media to downloaded in order to the links.
        /// </summary>
        public List<string> Extensions = new();
    }
    internal class GetRedditGallery
    {
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        /// <summary>
        /// Give in the string of the JSON and it lets out the links and extensions to each image in the Reddit Gallery!
        /// </summary>
        /// <param name="Json">String of all the data in the JSON</param>
        /// <returns>A FormattedLinks struct with ordered data with links and extensions.</returns>
        public static FormattedLinks FormatLinks(string Json)
        {
            FormattedLinks MediaData = new();

            List<string> _linksId = new();

            StringBuilder extensionTemp = new(""), extensionFinal = new("");

            int _mediaIdCount = 0;

            JObject _mediaIds, _mediaExtension;
            try
            {
                _mediaIds = new(

                        JObject.Parse(

                            JArray.Parse(Json)[0]["data"]["children"][0]["data"]["gallery_data"].ToString()
                        )
                    );

                _mediaExtension = new(

                        JObject.Parse(

                            JArray.Parse(Json)[0]["data"]["children"][0]["data"]["media_metadata"].ToString()
                        )
                    );

                try
                {
                    while (true)
                    {
                        _mediaIds["items"][_mediaIdCount]["media_id"].ToString();
                        _mediaIdCount++;
                    }
                }
                catch
                {
                    for (int i = 0; i < _mediaIdCount; i++)
                    {
                        _linksId.Add(_mediaIds["items"][i]["media_id"].ToString());
                    }

                    for (int i = 0; i < _linksId.Count; i++)
                    {
                        extensionTemp.Append(_mediaExtension[_linksId[i]]["m"].ToString());

                        if (extensionTemp.ToString().Contains("jpg"))
                        {
                            MediaData.Extensions.Add(".jpg");
                            extensionFinal.Append(".jpg");
                        }
                        else if (extensionTemp.ToString().Contains("png"))
                        {
                            MediaData.Extensions.Add(".png");
                            extensionFinal.Append(".png");
                        }
                        else if (extensionTemp.ToString().Contains("jpeg"))
                        {
                            MediaData.Extensions.Add(".jpeg");
                            extensionFinal.Append(".jpeg");
                        }

                        MediaData.Links.Add($"https://i.redd.it/{_linksId[i]}{extensionFinal}");

                        extensionFinal.Clear();
                    }
                    return MediaData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR PROCESSING GALLERY! \nEXCEPTION MESSAGE: {ex.Message} \n\n EXCEPTION STACK TRACE: {ex.StackTrace} \n\n INNER EXCEPTION: {ex.InnerException}");
            }
            return MediaData;

        }
        public static void GetGallery(string PathToResult, string sourceLink, FormattedLinks fLink, HttpClient _httpClient)
        {
            if (fLink.Links.Count > 0 && fLink.Extensions.Count > 0)
            {
                for (int i = 0; i < fLink.Links.Count; i++)
                {
                    if (!File.Exists(PathToResult + $"_GALLERY_IMAGE_{i}{fLink.Extensions[i]}"))
                    {
                        using FileStream fs0 = File.Create(PathToResult + $"_GALLERY_IMAGE_{i}{fLink.Extensions[i]}");

                        var _hrm = _httpClient.GetAsync(fLink.Links[i]).GetAwaiter().GetResult();

                        if (_hrm.IsSuccessStatusCode)
                            _hrm.Content.CopyToAsync(fs0).GetAwaiter().GetResult();

                        fs0.Dispose();
                        fs0.Close();
                        Console.WriteLine($"{Thread.CurrentThread.Name}; Downloaded Gallery from {sourceLink}");
                    }
                    else
                    {
                        InternalProgramData.TimesRepeated++;
                    }
                }
            }
        }
    }
}
