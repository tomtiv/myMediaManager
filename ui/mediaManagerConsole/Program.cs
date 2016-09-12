
namespace tomtiv.myMediaManager.ui.mediaManagerConsole
{
    using System;
    using System.Configuration;
    using core.mediaManagerLib;
    using NLog;

    class Program
    {

        private static Logger Applogger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            String folderToProcess = String.Empty;

            try
            {
                folderToProcess = args.Length > 0 ? args[0] : ConfigurationManager.AppSettings["foldertoprocess"];
            }
            catch (Exception)
            {
                Console.WriteLine("");
                Console.WriteLine("No path to process was provided");
                Console.WriteLine("update app.config or enter the path as a parameter");
                Console.WriteLine("");
                Console.WriteLine("Press enter to exit");
                Console.Read();
            }

            try
            {
                MediaItems mediaItems = new MediaItems {PathToProcess = folderToProcess};
                mediaItems.Load();
                mediaItems.ProcessItems();

                Console.WriteLine("--------- STARTING -------- ");

                foreach (MediaItem item in mediaItems.Items)
                {
                    if (item.Updated)
                    {
                        Console.WriteLine("oName: {0}", item.FileName);
                        Console.WriteLine("NName: {0}", item.NewFileName);
                        Console.WriteLine("");
                    }
                }

                Console.WriteLine("--------- DONE -------- ");
                Console.WriteLine("");

                Console.WriteLine("{0} items were processed", mediaItems.Items.Count);
                Console.WriteLine("{0} items were updated", mediaItems.ItemsUpdated);
                Console.WriteLine("{0} items were skipped", mediaItems.ItemsSkipped);
                Console.WriteLine("{0} items have errors", mediaItems.ItemsHaveErrors);
                Console.WriteLine("");

                if (mediaItems.ItemsUpdated == 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("There were no Updates");
                    Console.WriteLine("Press enter to exit");
                    Console.Read();
                    return;
                }

                Console.Write("Would you like to save changes - yes/No ");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    mediaItems.SaveChanges();

                    foreach (MediaItem item in mediaItems.Items)
                    {
                        if (item.Saved)
                        {
                            Console.WriteLine(@"Updated Name: {0}", item.NewFileName);
                        }
                    }

                    if (mediaItems.ItemsHaveDuplicates > 0)
                    {
                        Console.WriteLine("");
                        Console.Write("Delete all duplicates - yes/No ");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            mediaItems.RemoveDuplicates();
                        }
                    }

                    if (mediaItems.ItemsHaveErrors > 0)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Theses Items caused an error");

                        foreach (MediaItem item in mediaItems.Items)
                        {
                            if (item.HasError)
                            {
                                Console.WriteLine("File Name: {0}", item.FileName);
                                Console.WriteLine("Error: {0}", item.ErrorMessage);
                            }
                        }
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("Press enter to exit");
                Console.Read();
            }
            catch (MediaItemException mediaItemException)
            {
                Console.WriteLine(mediaItemException.Message);
                Console.WriteLine("");
                Console.WriteLine("Press enter to exit");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("");
                Console.WriteLine("Press enter to exit");
                Console.Read();
            }
        }
    }
}
