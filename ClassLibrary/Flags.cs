using ClassLibrary.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ClassLibrary.APINetwork;
using ProjetoFinal_API;

namespace ClassLibrary
{
    public class Flags
    {
        private string? _png;

        private string? _svg;

        private string? _localImage;

        private Root _roots;

        public string Png
        {
            get
            {
                if (_png == null || _png.Length == 0)
                    return "pack://application:,,,/Imagens/no_flag.png";

                return _png;
            }
            set
            {
                _png = value;
            }
        }
        public string Svg
        {
            get
            {
                if (_svg == null || _svg.Length == 0)
                    return "pack://application:,,,/Imagens/no-flag.svg";

                return _svg;
            }
            set
            {
                _svg = value;
            }
        }
        public string LocalImage { get; set; } = "pack://application:,,,/Imagens/no_flag.png";
        public string ShowFlags
        {
            get
            {
                string flagPath = Directory.GetCurrentDirectory() + @"/Flags/" + $"{_roots.CCA3}.png";

                if (File.Exists(flagPath))
                    return LocalImage;

                if (!NetworkService.IsAvailable)
                    return "pack://application:,,,/Imagens/no_flag.png";

                if (Png == null || Png.Length == 0)
                    return "pack://application:,,,/Imagens/no_flag.png";

                return Png;
            }
        }

        public Flags(Root bandeira)
        {
            _roots = bandeira;
        }

    }
}
