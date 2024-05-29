namespace ClassLibrary
{
    public class Name
    {
       public string official { get; set; }

        private string _official;
        private Dictionary<string, Name> _names = new Dictionary<string, Name>();

        public string Official
        {
            get
            {
                if (official == null || official.Length == 0)
                    return "N/A";

                return official;
            }
            set
            {
                official = value;
            }
        }
        public Dictionary<string, Name> Names
        {
            get
            {
                if (_names.Count > 1 && _names.First().Key == "default")
                    _names.Remove("default");

                return _names;
            }

            set
            {
                _names = value;
            }
        }

    }
}
