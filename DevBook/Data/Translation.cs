namespace DevBook.Data
{
    public struct Translation
    {
        public Word Target { get; set; }
        public Word Native { get; set; }

        public override string ToString()
        {
            return $"\'{Target.Value}\' ({Target.Language}) = \'{Native.Value}\' ({Native.Language})";
        }
    }
}