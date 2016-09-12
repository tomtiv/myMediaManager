namespace tomtiv.myMediaManager.core.mediaManagerLib
{
    using System;

    public class MediaItem
    {        
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string NewFileName { get; set; }
        public bool Updated { get; set; }
        public bool IsDuplicate { get; set; }
        public bool HasError { get; set; }
        public bool Skipped { get; set; }
        public string ErrorMessage { get; set; }
        public bool Saved { get; set; }
        public MediaItemException mediaItemException { get; set; }
    }
}
