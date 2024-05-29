using ClassLibrary;

namespace ProjetoFinal_API
{
    public class Root
    {
        public Name _name;
        private List<string> _capital;
        private string _region;
        private string _subregion;
        private int _population;
        private string[] _continents;
        private Flags _flags;
        private string _cca3;
        private Dictionary<string, string> _gini;


        public Name name
        {
            get { return _name; }
            set { _name = value ?? new Name(); }
        }

        public List<string> capital
        {
            get { return _capital; }
            set { _capital = value ?? new List<string>(); }
        }

        public string region
        {
            get { return _region; }
            set { _region = value ?? string.Empty; }
        }

        public string subregion
        {
            get { return _subregion; }
            set { _subregion = value ?? string.Empty; }
        }

        public int population
        {
            get { return _population; }
            set { _population = value; }
        }

        public string[] continents
        {
            get
            {
                if (_continents.Length == 0)
                {
                    return new string[1] { "N/A" };
                }
                return _continents;
            }

            set { _continents = value; }
        }

        public string CCA3
        {
            get
            {
                if (_cca3 == null || _cca3.Length == 0)
                    return "N/A";

                return _cca3;
            }
            set
            {
                _cca3 = value;
            }
        }

        public Flags Flags { get; set; }

        public override string ToString()
        {
            string countries = $"{_gini}";
            return countries;
        }

        public Dictionary<string, string> Gini
        {
            get
            {
                if (_gini.Count > 1 && _gini.First().Key == "default")
                    _gini.Remove("default");

                return _gini;
            }

            set
            {
                _gini = value;
            }
        }


        public Root()
        {
            name = new Name();
            name.Names = new Dictionary<string, Name>
        {
            { "default", new Name { Official = "N/A" } }
        };

            Flags = new Flags(this);

            capital = new List<string>();

            continents = new string[0];

            Gini = new Dictionary<string, string>()
            {
                { "default", "N/A"}
            };

        }
    }

}
