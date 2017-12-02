using System.Collections.Generic;
using System.Linq;

namespace BCNF
{

    class Atribut
    {
        public string content { get; set; }

        public Atribut(string s)
        {
            content = s;
        }

        public override string ToString()
        {
            return content;
        }
    }

    class Skup_Atributa
    {
        public Skup_Atributa(Skup_Atributa other) // "Deep copy" konstruktor
        {
            skup = new List<Atribut>();
            foreach (Atribut a in other.skup)
            {
                skup.Add(a) ;
            }
        }
        public Skup_Atributa()
        {
            skup = new List<Atribut>();
        }

        public override string ToString()
        {
            return string.Join(", ", skup);
        }

        public string MyStr()
        {
            return string.Join("", skup);
        }

        public List<Atribut> skup { get; set; }

        public bool Contains(Atribut atribut_za_provjeru) //Novi Contains
        {   
            int flag = 0;
            foreach (Atribut a in skup)
            {
                if (a.content == atribut_za_provjeru.content)
                {
                    flag = 1;
                }
            }
            if (flag!=0)
            {
                return true;
            }
            return false;
        }
    }

    class FO
    {
        public FO()
        {
            Lijevo = new Skup_Atributa();
            Desno = new Skup_Atributa();
        }
        public Skup_Atributa Lijevo { get; set; }
        public Skup_Atributa Desno { get; set; }

        public override string ToString()
        {
            string l = string.Join(", ", Lijevo.skup);
            string d = string.Join(", ", Desno.skup);
            return '{' + l + " -> " + d + '}';
        }

        public string MyStr()
        {
            string l = string.Join("|", Lijevo.skup);
            string d = string.Join("|", Desno.skup);
            return l + "->" + d;
        }
    }

    class Problem
    {
        public Problem()
        {
            veliko_R = new Skup_Atributa();
            relacije = new List<FO>();
            kljucevi = new List<Skup_Atributa>();
        }

        public Skup_Atributa veliko_R { get; set; }
        public List<Skup_Atributa> kljucevi { get; set; }
        public List<FO> relacije { get; set; }
        MainWindow myWin = ((MainWindow)System.Windows.Application.Current.MainWindow);

        public bool Contains(Skup_Atributa potencijalni_kljuc)
        {
            foreach (Skup_Atributa sa in kljucevi)
            {
                int brojac = 0;
                foreach (Atribut a in sa.skup)
                {
                    foreach (Atribut a_pk in potencijalni_kljuc.skup)
                    {
                        if (a.content == a_pk.content)
                        {
                            brojac++;
                        }
                    }
                }
                if (brojac == sa.skup.Count())
                {
                    return true;
                }
            }
            return false;
        }

        public void Pronadji_Kljuceve()
        {
            myWin.debugBox.Text = "";
            kljucevi.Clear();
            pronadi_kljuceve_rekurzija(veliko_R);
        }

        bool pronadi_kljuceve_rekurzija(Skup_Atributa potencijalni_kljuc)
        { 
            int flag = 0;
            //gleda jeli ograda kljuca pokriva R
            if (!provjeri_kljuc(potencijalni_kljuc))
            {
                return false;
            }
            List<Skup_Atributa> dijelovi_kljuca = new List<Skup_Atributa>();
            int velicina = potencijalni_kljuc.skup.Count;
            //razbija kljuc u podkljuceve
            for (int i = 0; i < velicina; i++)
            {
                Skup_Atributa temp = new Skup_Atributa();
                for (int j = 0; j < velicina - 1; j++)
                {
                    int offset = (i + j) % velicina;
                    temp.skup.Add(new Atribut(potencijalni_kljuc.skup[offset].content));
                }
                temp.skup = temp.skup.OrderBy(o => o.content).ToList();
                dijelovi_kljuca.Add(temp);
            }
            foreach (Skup_Atributa sa in dijelovi_kljuca)
            {
                //provjerava za dijelove kljuca jesu li vec kljc, ako jesu stavlja flag na 1
                if (Contains(sa))
                {
                    flag = 1;
                }
                //provjerava rekurzivno za dijelove kljuca jeli pokrivaju R, ako jesu stavlja flag na 1
                else if (pronadi_kljuceve_rekurzija(sa))
                {
                    flag = 1;
                }
            }
            //ako je flag 1 vraca true sto znaci da ovaj kljuc nije minimalan niti njegovi nadkljucevi ili vec postoji
            if (flag == 1)
            {
                return true;
            }

            kljucevi.Add(potencijalni_kljuc);
            dijelovi_kljuca.Clear();

            return true;
        }
         
        bool provjeri_kljuc(Skup_Atributa potencijalni_kljuc)
        {
            myWin.Append("Ograda: "+potencijalni_kljuc.ToString());

            Skup_Atributa temp = new Skup_Atributa(potencijalni_kljuc);//CP konstruktor(RADI!!)
            int brojac = 0;
            int size = potencijalni_kljuc.skup.Count();
            while (size >= 0)
            { 
                foreach (FO fo in relacije)
                {
                    foreach (Atribut at_left in fo.Lijevo.skup)
                    {
                        foreach (Atribut atributi_kljuca in temp.skup)
                        {
                            if (at_left.content == atributi_kljuca.content)
                                brojac++;
                
                        }
                    }
                    
                    if (brojac == fo.Lijevo.skup.Count())
                    {
                        foreach (Atribut at_right in fo.Desno.skup)
                        {   
                            Atribut novi = new Atribut(at_right.content);
                            
                            if (!temp.Contains(novi))
                            {
                                temp.skup.Add(novi);
                            }
                        }
                    }
                    brojac = 0;
                }
                size--;
            }

            if (temp.skup.Count() >= veliko_R.skup.Count())
            {
                return true;
            }
            return false;
        }
    }
}
