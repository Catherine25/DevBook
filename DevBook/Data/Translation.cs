namespace DevBook.Data
{
    public struct Translation
    {
        public Translation(Word target, Word native)
        {
            Target = target;
            Native = native;
        }

        public Word Target { get; set; }
        public Word Native { get; set; }

        public override string ToString()
        {
            return $"\'{Target.Value}\' ({Target.Language}) = \'{Native.Value}\' ({Native.Language})";
        }
    }
}