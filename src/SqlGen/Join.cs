namespace SqlGen
{
    internal sealed class Join
    {
        public string Expression { get; set; }

        public JoinType JoinType { get; set; }

        public Query Table1 { get; set; }

        public Query Table2 { get; set; }
    }
}
