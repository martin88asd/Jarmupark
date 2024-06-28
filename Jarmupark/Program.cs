using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarmupark
{
    abstract class Jarmu
    {
        // tulajdonságok (Properties)
        public string Azonosito { get; private set; }
        public string Rendszam { get; set; }
        public int GyartasiEv { get; private set; }
        public double Fogyasztas { get; set; }
        public double Futottkm { get; private set; }
        public int AktualisKoltseg { get; private set; }
        public bool Szabad { get; private set; }
        public static int AktualisEv { get; set; }
        public static int AlapDij { get; set; }
        public static double Haszonkulcs { get; set; }
        // konstruktor (ha már ismerjük a fogyasztást)
        public Jarmu(string azonosito, string rendszam, int gyartasiEv, double fogyasztas)
        {
            this.Azonosito = azonosito;
            this.Rendszam = rendszam;
            this.GyartasiEv = gyartasiEv;
            this.Fogyasztas = fogyasztas;
            this.Szabad = true;

        }
        // konstruktor (ha még nem ismerjük a fogyasztást, pl. vadonatúj a jármű) public Jarmu (string azonosito, string rendszam, int gyartasiEv) {
        public Jarmu(string azonosito, string rendszam, int gyartasiEv)
        {
            this.Azonosito = azonosito;
            this.Rendszam = rendszam;
            this.GyartasiEv = gyartasiEv;
            this.Szabad = true;
        }


        // azért nem a Foglalt tulajdonságot vezettük be, hogy ezt az értékadást //is megmutathassuk.
        // metódusok
        /// <summary>
        // Kiszámolja a jármű korát.
        // </summary>
        // <returns></returns>
        public int Kor()
        {
            return AktualisEv - GyartasiEv;
        }

        public bool Fuvaroz(double ut, int benzinAr)
        {
            if (Szabad)
            {
                Futottkm += ut;
                AktualisKoltseg = (int)(benzinAr * ut * Fogyasztas / 100);
                Szabad = false;
                return true;
            }
            return false;
        }

        public virtual int BerletDij()
        {
            return (int)(AlapDij + AktualisKoltseg + AktualisKoltseg * Haszonkulcs / 100);
        }

        public void Vegzett()
        {
            Szabad = true;
        }

        public override string ToString()
        {
            return "\nA" + this.GetType().Name.ToLower() +
                " azonosítója: " + Rendszam +
                "\n         kora: " + Kor() + " év" +
                "\n         fogyasztása: " + Fogyasztas + "  l/100 km" +
                "\n         a km-óra álláasa: " + Futottkm + " km";
        }

    }

    class Busz : Jarmu
    {
        public int Ferohely { get; private set; }
        public static double Szorzo { get; set; }

        public Busz(string azonosito, string rendszam, int gyartasiEv, double fogyasztas, int ferohely) :
            base(azonosito, rendszam, gyartasiEv, fogyasztas)
        {
            this.Ferohely = ferohely;
        }

        public Busz(string azonosito, string rendszam, int gyartasiEv, int ferohely) : base(azonosito, rendszam, gyartasiEv)
        {
            this.Ferohely = ferohely;
        }

        public override int BerletDij()
        {
            return (int)(base.BerletDij() + Ferohely * Szorzo);
        }

        public override string ToString()
        {
            return base.ToString() + "\nferőhely száma " + Ferohely;
        }
    }

    class Teherauto : Jarmu
    {
        public double TeherBiras { get; private set; }
        public static double Szorzo { get; set; }

        public Teherauto(string azonosito, string rendszam, int gyartasiEv, double fogyasztas, double teherBiras) : base(azonosito, rendszam, gyartasiEv, fogyasztas)
        {
            this.TeherBiras = TeherBiras;
        }

        public Teherauto(string azonosito, string rendszam, int gyartasiEv, double teherBiras) : base(azonosito, rendszam, gyartasiEv)
        {
            this.TeherBiras = TeherBiras;
        }

        public override int BerletDij()
        {
            return (int)(base.BerletDij() + TeherBiras * Szorzo);
        }

        public override string ToString()
        {
            return base.ToString() + "\n\tteherbírás: " + TeherBiras + " tonna";
        }

    }

    class Vezerles
    {
        private List<Jarmu> jarmuvek = new List<Jarmu>();
        private string BUSZ = "busz";
        private string TEHER_AUTO = "teherautó";

        public void Indit()
        {
            StatikusBeallitas();
            AdatBevitel();
            Kiir("A regisztrált járművek: ");
            AtlagKor();
            LegtobbKilometer();
            Rendez();
        }

        private void StatikusBeallitas()
        {
            Jarmu.AktualisEv = 2015;
            Jarmu.AlapDij = 1000;
            Jarmu.Haszonkulcs = 10;

            Busz.Szorzo = 15;
            Teherauto.Szorzo = 8.5;
        }

        private void AdatBevitel()
        {
            string tipus, rendszam, azonosito;
            int gyarEv, ferohely;
            double fogyasztas, teheriras;

            StreamReader olvasoCsatorna = new StreamReader("jarmuvek.txt");

            int sorszam = 1;

            while (!olvasoCsatorna.EndOfStream)
            {
                tipus = olvasoCsatorna.ReadLine();
                Console.WriteLine(tipus);
                rendszam = olvasoCsatorna.ReadLine();
                gyarEv = int.Parse(olvasoCsatorna.ReadLine());
                fogyasztas = double.Parse(olvasoCsatorna.ReadLine());
                azonosito = tipus.Substring(0, 1).ToUpper() + sorszam;

                if (tipus == BUSZ)
                {
                    ferohely = int.Parse(olvasoCsatorna.ReadLine());
                    jarmuvek.Add(new Busz(azonosito, rendszam, gyarEv, fogyasztas, ferohely));
                }
                else if (tipus == TEHER_AUTO)
                {
                    teheriras = double.Parse(olvasoCsatorna.ReadLine());
                    jarmuvek.Add(new Teherauto(azonosito, rendszam, gyarEv, fogyasztas, teheriras));
                }
                sorszam++;
            }
            olvasoCsatorna.Close();
        }
        private void Kiir(string cim)
        {
            Console.WriteLine(cim);
            foreach (Jarmu jarmu in jarmuvek)
            {
                Console.WriteLine(jarmu);
            }
        }

        private void Mukodtet() 
        {
            int osszKoltseg = 0, osszBevetel = 0;

            Random rand = new Random();
            int alsoBenzinAr = 400, felsoBenzinAr = 420;
            double utMax = 300;
            int mukodesHatar = 200;
            int jarmuIndex;

            Jarmu jarmu;
            int fuvarSzam = 0;

            for (int i = 0; i < (int)rand.Next(mukodesHatar); i++)
            {
                jarmuIndex = rand.Next(jarmuvek.Count);
                jarmu = jarmuvek[jarmuIndex];
                if (jarmu.Fuvaroz(rand.NextDouble() - utMax, rand.Next(alsoBenzinAr, felsoBenzinAr)))
                {
                    fuvarSzam++;
                    Console.WriteLine("\nAz induló Jármű rendszáma: " + jarmu.Rendszam 
                        + "\nAz aktuális fuvarozási költség: " 
                        + jarmu.AktualisKoltseg + " Ft." 
                        + "\nAz aktuális bevétel: " 
                        + jarmu.BerletDij() + " Ft");

                    osszBevetel += jarmu.BerletDij();
                    osszKoltseg += jarmu.AktualisKoltseg;
                }

                jarmuIndex = rand.Next(jarmuvek.Count);
                jarmuvek[jarmuIndex].Vegzett();
            }

            Console.WriteLine("\n\nA cég teljes költsége: " + osszKoltseg 
                + " Ft" +
                "\n\nTeljes bevétel" + osszBevetel + " Ft" + 
                "\n\nHaszna: " + (osszBevetel-osszKoltseg) + " Ft");
            Console.WriteLine("\nA fuvatok száma: " + fuvarSzam);
        }

        private void AtlagKor()
        {
            double osszkor = 0;
            int darab = 0;
            foreach (Jarmu jarmu in jarmuvek)
            {
                osszkor += jarmu.Kor();
                darab++;
            }

            if (darab > 0)
            {
                Console.Write("\n A járművek átlag kora: " + osszkor / darab + " év");
            }
            else
            {
                Console.WriteLine("\nNincsenek járművek");
            }
        }

        private void LegtobbKilometer()
        {
            double max = jarmuvek[0].Futottkm;
            foreach (Jarmu jarmu in jarmuvek)
            {
                if (jarmu.Futottkm == max)
                {
                    Console.WriteLine(jarmu.Rendszam);
                }
            }
        }

        private void Rendez()
        {
            Jarmu temp;

            for (int i = 0; (i < jarmuvek.Count-1); i++)
            {
                for (int j = 0; (j < jarmuvek.Count); j++)
                {
                    if (jarmuvek[i].Fogyasztas > jarmuvek[j].Fogyasztas) { }
                    {
                        temp = jarmuvek[i];
                        jarmuvek[i] = jarmuvek[j];
                        jarmuvek[j] = temp;
                    }

                }
            }
            Console.WriteLine("\nA járművek fogyasztás szerint rendezve: ");

            foreach (Jarmu jarmu in jarmuvek)
            {
                Console.WriteLine("{0,-10} {1:00:0} liter / 100km.", jarmu.Rendszam, jarmu.Fogyasztas);
            }
        }
    }

        internal class Program
        {
            static void Main(string[] args)
            {
                Vezerles vezerles = new Vezerles();
                vezerles.Indit();
                Console.ReadKey();
            }
        }
}