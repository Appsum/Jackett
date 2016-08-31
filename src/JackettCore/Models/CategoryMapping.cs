namespace JackettCore.Models
{
    class CategoryMapping
    {
        public CategoryMapping(string trackerCat, int newzCat)
        {
            TrackerCategory = trackerCat;
            NewzNabCategory = newzCat;
        }

        public string TrackerCategory { get; private set; }
        public int NewzNabCategory { get; private set; }
    }
}
