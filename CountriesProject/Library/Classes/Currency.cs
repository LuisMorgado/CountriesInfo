namespace Library.Classes
{
    public class Currency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string CountryCode { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
