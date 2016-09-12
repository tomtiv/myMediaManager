namespace tomtiv.myMediaManager.core.mediaManagerLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Linq;
    using NLog;

    public class MediaItems
    {
        private static Logger Liblogger = LogManager.GetCurrentClassLogger();

        List<MediaItem> mediaItems = new List<MediaItem>();

        private String[] keywordList;
        private String[] regExList;

        public String PathToProcess { get; set; }
        public int ItemsUpdated { get; set; }
        public int ItemsSkipped { get; set; }
        public int ItemsHaveErrors { get; set; }
        public int ItemsHaveDuplicates { get; set; }

        public List<MediaItem> Items => this.mediaItems;

        public void Load()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(PathToProcess);

                foreach (FileInfo file in dir.GetFiles())
                {
                    MediaItem mediaItem = new MediaItem
                    {
                        FilePath = file.FullName.Replace(file.Name, String.Empty),
                        NewFileName = file.Name.Replace(file.Extension.ToLower(), String.Empty),
                        FileName = file.Name.Replace(file.Extension.ToLower(), String.Empty),
                        FileExt = file.Extension.ToLower()
                    };
                    mediaItems.Add(mediaItem);
                }
            }
            catch (DirectoryNotFoundException directoryNotFound)
            {
                Liblogger.Fatal(directoryNotFound.Message);
                throw new MediaItemException(directoryNotFound.Message, directoryNotFound.InnerException);

            }
            catch (System.Exception sysException)
            {
                throw new MediaItemException(sysException.Message, sysException.InnerException);
            }

            try
            {
                keywordList = File.ReadAllText("keywords.txt").Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            }
            catch (FileNotFoundException fileNotFound)
            {
                throw new MediaItemException(fileNotFound.Message, fileNotFound.InnerException);

            }
            catch (System.Exception sysException)
            {
                throw new MediaItemException(sysException.Message, sysException.InnerException);
            }

            try
            {
                regExList = File.ReadAllText("regEx.txt").Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            }
            catch (FileNotFoundException fileNotFound)
            {
                throw new MediaItemException(fileNotFound.Message, fileNotFound.InnerException);

            }
            catch (System.Exception sysException)
            {
                throw new MediaItemException(sysException.Message, sysException.InnerException);
            }
        }

        public void ProcessItems()
        {
            foreach (MediaItem item in mediaItems)
            {
                if (!item.Skipped && !item.HasError)
                {
                    RemoveKeywords(item);
                    FixFormatting(item);
                    //ProcessUDR(item);
                }

                CheckForChanges(item);
            }
        }

        private void FixFormatting(MediaItem item)
        {
            try
            {
                // Remove Track Numbers
                item.NewFileName = Regex.Replace(item.NewFileName, @"^[\d]{2,}\s*\-", String.Empty, RegexOptions.IgnoreCase);
                item.NewFileName = Regex.Replace(item.NewFileName, @"\s*[\d]{2,}\s*", " ", RegexOptions.IgnoreCase);

                //TODO:  FIX hedley-stormy 

                // Remove Special Chars
                item.NewFileName = Regex.Replace(item.NewFileName, @"\s*[\.]{2,}\s*", " ", RegexOptions.IgnoreCase);
                item.NewFileName = Regex.Replace(item.NewFileName, @"\s*[\-]{2,}\s*", " -", RegexOptions.IgnoreCase);
                item.NewFileName = item.NewFileName.Replace("[]", String.Empty);
                item.NewFileName = item.NewFileName.Replace("<>", String.Empty);
                item.NewFileName = item.NewFileName.Replace("()", String.Empty);
                item.NewFileName = item.NewFileName.Replace("?", String.Empty);

                // Fix Spacing
                item.NewFileName = Regex.Replace(item.NewFileName, @"[ ]{2,}", " - ", RegexOptions.None);
                item.NewFileName = Regex.Replace(item.NewFileName, @"\\s +", " ", RegexOptions.None);

                item.NewFileName = item.NewFileName.Replace("- - - -", " - ");
                item.NewFileName = item.NewFileName.Replace("- - -", " - ");
                item.NewFileName = item.NewFileName.Replace("- -", " - ");
                item.NewFileName = item.NewFileName.Replace("  ", " ");

                while (!Char.IsLetterOrDigit(item.NewFileName.First()))
                {
                    item.NewFileName = item.NewFileName.Trim(item.NewFileName.First());
                }

                while (!Char.IsLetterOrDigit(item.NewFileName.Last()))
                {
                    item.NewFileName = item.NewFileName.Trim(item.NewFileName.Last());

                    if (item.NewFileName.Last() == ']' || item.NewFileName.Last() == ')')
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                item.HasError = true;
                item.ErrorMessage = ex.Message;
                Liblogger.Warn(ex.Message);
            }
        }

        private void RemoveKeywords(MediaItem item)
        {
            try
            {
                foreach (String term in keywordList)
                {
                    item.NewFileName = Regex.Replace(item.NewFileName, @term, String.Empty, RegexOptions.IgnoreCase);
                }
            }
            catch (System.Exception ex)
            {
                item.HasError = true;
                item.ErrorMessage = ex.Message;
                Liblogger.Warn(ex.Message);
            }
        }

        private void ProcessUDR(MediaItem item)
        {
            try
            {
                foreach (String term in regExList)
                {
                    item.NewFileName = Regex.Replace(item.NewFileName, @term, String.Empty, RegexOptions.None);
                }
            }
            catch (System.Exception ex)
            {
                item.HasError = true;
                item.ErrorMessage = ex.Message;
            }
        }

        private void CheckForChanges(MediaItem item)
        {
            if (String.CompareOrdinal(item.FileName, item.NewFileName) != 0)
            {
                item.Updated = true;
                ItemsUpdated++;
            }
        }

        public void SaveChanges()
        {
            foreach (MediaItem item in mediaItems)
            {
                item.Saved = false;

                if (item.Skipped || item.HasError || !item.Updated) continue;
                try
                {
                    String oFile = $"{item.FilePath}{item.FileName}{item.FileExt}";
                    String nFile = $"{item.FilePath}{item.NewFileName}{item.FileExt}";
                    System.IO.File.Move(oFile, nFile);
                    item.Saved = true;
                }

                catch (FileNotFoundException fex)
                {
                    item.HasError = true;
                    item.ErrorMessage = fex.Message;
                    Liblogger.Warn(fex.Message);
                }

                catch (Exception ex)
                {
                    item.HasError = true;
                    item.ErrorMessage = ex.Message;
                    if(ex.Message.Contains("already exists"))
                    {
                        item.IsDuplicate = true;
                        ItemsHaveDuplicates++;
                    }

                    Liblogger.Warn(ex.Message);
                }
            }
        }

        public void RemoveDuplicates()
        {
            try
            {
                foreach (MediaItem item in mediaItems)
                {
                    try
                    {
                        if (item.IsDuplicate)
                        {
                            String oFile = $@"{item.FilePath}{item.FileName}{item.FileExt}";
                            System.IO.File.Delete(@oFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        String oFile = $"{item.FilePath}{item.FileName}{item.FileExt}";
                        String nFile = $"{item.FilePath}{item.NewFileName}DUP_{item.FileExt}";
                        System.IO.File.Move(oFile, nFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Liblogger.Warn(ex.Message);
                MediaItemException mediaItemException = new MediaItemException(ex.Message, ex.InnerException);
            }
        }
    }
}