using Microsoft.Win32;
using System;
using System.Collections;
using System.Linq;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using System.Globalization;

namespace SWD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<List<object>> listaKolumn = new List<List<object>>();
        DataTable daneTab = new DataTable();
        String naglowki;

        public MainWindow()
        {
            InitializeComponent();
        }

        public class ItemObject
        {
            string Text;
            int Value;

            public ItemObject(string s, int i)
            {
                Text = s;
                Value = i;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void wgraj_Click(object sender, RoutedEventArgs e)
        {
            listaKolumn = new List<List<object>>();
            daneTab = new DataTable();
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog(); 
                             
                if (result == true)
                {
                    string filename = dlg.FileName;
                    

                    // Wpisanie do naszej listy || Lista list obiektów = mamy na każdą kolumnę jako 
                    // listę typu Object i te kolumny siedzą w liscie listaKolumn, 
                    // jak chcesz się dostać do wiersza 10, w 5 kolumnie to dajesz listaKolumn[5][10]
                    foreach (string k in File.ReadAllLines(filename))
                    {
                        if (k.StartsWith("#"))
                            continue;
                        if (k == "")
                            continue;
                        if (k.StartsWith("!"))
                        {
                            naglowki = k.TrimStart('!');
                            continue;
                        }

                        int j = 0;
                        // foreach dla każdego stringa wydzielonego z jednej przeczytaniej linii, stringa dzieli spacja lub przecinek
                        foreach (string l in k.Split(new char[] { ' ', '\t' , ';'}))
                        {
                            // dodanie list reprezentujących kolumny, jednorazowo przy pierwszej iteracji
                            if(listaKolumn.Count < k.Split(new char[] { ' ', '\t', ';' }).Length)
                            {
                                listaKolumn.Add(new List<object>());
                            }
                            double temp;
                            // jeżeli da się rzutować jedno pole z pliku tekstowego na liczbę to wpisz liczbę jak nie to stringa
                            if (Double.TryParse(l.Replace('.', ','), out temp))
                                listaKolumn[j].Add(temp);
                            else
                                listaKolumn[j].Add(l);
                            j++;                            
                        }
                    }                   

                    //DataTable dane = new DataTable();

                    if (naglowki != null)
                    {
                        int i = 0;
                        foreach (string l in naglowki.Split(new char[] { ' ', '\t', ';' }))
                        {                            
                            double temp;
                            // Rzutowanie jak przy tworzeniu list, jak liczba to typu double jak nie to String
                            if (Double.TryParse(listaKolumn[i][0].ToString(), out temp))
                                daneTab.Columns.Add(l, typeof(Double));
                            else
                                daneTab.Columns.Add(l, typeof(String));
                            i++;
                        }
                    }
                    else
                        for (int i = 0; i < listaKolumn.Count; i++)
                        {
                            double temp;
                            // Rzutowanie jak przy tworzeniu list, jak liczba to typu double jak nie to String
                            if (Double.TryParse(listaKolumn[i][0].ToString(), out temp))
                                daneTab.Columns.Add("Kolumna" + (i + 1), typeof(Double));
                            else
                                daneTab.Columns.Add("Kolumna" + (i + 1), typeof(String));
                        }

                    // Stworzenie kolumn w DataTable
                    
                    // Wpisanie danych do DataTable pierwszy for po wierszach, wewnętrzny po kolumnach
                    for (int i = 0; i < listaKolumn[0].Count; i++)
                    {
                        DataRow row = daneTab.Rows.Add();
                        for (int j = 0; j < listaKolumn.Count; j++)
                            row[j] = listaKolumn[j][i];
                    }
                    
                    // Zbindowanie danych z DataTable do GridView
                    blok.ItemsSource = daneTab.AsDataView();


                    // Wpisanie kolumn do listy rozwijanej

                    wybor.Items.Clear();
                    wybor_chart1.Items.Clear();
                    wybor_chart2.Items.Clear();

                    int z = 0;
                    foreach (var x in daneTab.Columns)
                    {
                        wybor.Items.Add(new ItemObject(x.ToString(), z));
                        wybor_chart1.Items.Add(new ItemObject(x.ToString(), z));
                        wybor_chart2.Items.Add(new ItemObject(x.ToString(), z));
                        wybor_chart_klasa.Items.Add(new ItemObject(x.ToString(), z));
                        z++;
                    }

                    wybor.SelectedIndex = 0;
                    wybor_chart1.SelectedIndex = 0;
                    wybor_chart2.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Niepoprawny plik");
                throw;
            }
        }

        private void odswiezlisty()
        {
            wybor.Items.Clear();
            wybor_chart1.Items.Clear();
            wybor_chart2.Items.Clear();
            wybor_chart_klasa.Items.Clear();

            int i = 0;
            foreach (var x in daneTab.Columns)
            {
                wybor.Items.Add(new ItemObject(x.ToString(), i));
                wybor_chart1.Items.Add(new ItemObject(x.ToString(), i));
                wybor_chart2.Items.Add(new ItemObject(x.ToString(), i));
                wybor_chart_klasa.Items.Add(new ItemObject(x.ToString(), i));
                i++;
            }

            wybor.SelectedIndex = 0;
            wybor_chart1.SelectedIndex = 0;
            wybor_chart2.SelectedIndex = 0;
        }

        private void wybor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (wybor.SelectedIndex == -1)
                return;
            try
            {
                text_średnia.Text = srednia() + "";
                text_mediana.Text = mediana() + "";
                Double[] mm1 = minmax();
                text_minmax1.Text = mm1[0] + "";
                text_minmax2.Text = mm1[1] + "";
                Double[] q1 = kwartyle();
                text_kw1.Text = q1[0] + "";
                text_kw2.Text = q1[1] + "";
                Double[] p1 = percentyle();
                text_p1.Text = p1[0] + "";
                text_p2.Text = p1[1] + "";
                text_p3.Text = p1[2] + "";
                text_p4.Text = p1[3] + "";
            }
            catch (InvalidCastException ex)
            {
                text_średnia.Text = "Nie liczbowa kolumna";
                //throw;
            }
        }

        public Double srednia()
        {
            Double cnt = 0;
            for (int i = 0; i < listaKolumn[0].Count; i++)
            {
                try
                {
                    cnt += (Double)listaKolumn[wybor.SelectedIndex][i];
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            cnt = (cnt / listaKolumn[wybor.SelectedIndex].Count);
            return cnt;
        }

        private double wariancja()
        {
            double war=0;
            double avg = srednia();
            foreach (DataRow r in daneTab.Rows)
            {
                war += Math.Pow(((double)r[wybor.SelectedIndex] - avg), 2);
            }

            war = war / daneTab.Rows.Count;
            return war;
        }

        public Double mediana()
        {
            Double med = 0;
            int liczba_wierszy = listaKolumn[wybor.SelectedIndex].Count;
            int a = liczba_wierszy / 2;


            listaKolumn[wybor.SelectedIndex].Sort();

            if (liczba_wierszy % 2 == 0)
            {
                med = ((Double)listaKolumn[wybor.SelectedIndex][a - 1] + (Double)listaKolumn[wybor.SelectedIndex][a]) / 2;
            }
            else
            {
                med = (Double)listaKolumn[wybor.SelectedIndex][a - 1];
            }

            return med;
        }

        public Double[] minmax()
        {
            Double[] mm = new Double[2];
            int liczba_wierszy = listaKolumn[wybor.SelectedIndex].Count;

            listaKolumn[wybor.SelectedIndex].Sort();

            mm[0] = (Double)listaKolumn[wybor.SelectedIndex][0];
            mm[1] = (Double)listaKolumn[wybor.SelectedIndex][liczba_wierszy - 1];

            return mm;

        }

        public Double[] kwartyle()
        {

            Double[] q = new Double[2];
            int liczba_wierszy = listaKolumn[wybor.SelectedIndex].Count;
            double a = (double)(liczba_wierszy / 4);

            listaKolumn[wybor.SelectedIndex].Sort();

            q[0] = (double)listaKolumn[wybor.SelectedIndex][(int)a];
            q[1] = (double)listaKolumn[wybor.SelectedIndex][(int)(a * 3)];

            return q;
        }

        public Double[] percentyle()
        {

            Double[] p = new Double[4];
            double liczba_wierszy = listaKolumn[wybor.SelectedIndex].Count;
            double a = liczba_wierszy / (double)100;

            listaKolumn[wybor.SelectedIndex].Sort();

            p[0] = (double)listaKolumn[wybor.SelectedIndex][(int)(5 * a)];
            p[1] = (double)listaKolumn[wybor.SelectedIndex][(int)(10 * a)];
            p[2] = (double)listaKolumn[wybor.SelectedIndex][(int)(90 * a)];
            p[3] = (double)listaKolumn[wybor.SelectedIndex][(int)(95 * a)];

            return p;
        }
        // Dyskretyzacja przedziały
        private void dyskr_Click(object sender, RoutedEventArgs e)
        {
            int liczPrzedz = Int32.Parse(przedzial.Text);
            double dlugosc;

            double[] minMax = minmax();
            double[] przedzialCur = new double[2];
            
            dlugosc = (minMax[1] - minMax[0]) / liczPrzedz;

            przedzialCur[0] = minMax[0];
            przedzialCur[1] = przedzialCur[0];

            daneTab.Columns.Add("DYS_K" + wybor.SelectedIndex + "_P" + liczPrzedz, typeof(int));

            for (int i = 0; i < liczPrzedz; i++)
            {
                if(i!=0)
                    przedzialCur[0] += dlugosc;
                przedzialCur[1] += dlugosc;
                foreach (DataRow row in daneTab.Rows)
                {
                    if ((double)row[wybor.SelectedIndex] >= przedzialCur[0] &&  (double)row[wybor.SelectedIndex] < przedzialCur[1])
                    {
                        row[row.ItemArray.Length - 1] = i + 1;
                    }
                    if(i==liczPrzedz-1 && (double)row[wybor.SelectedIndex] == minMax[1])
                        row[row.ItemArray.Length - 1] = i + 1;
                }
                
            }
            blok.ItemsSource = daneTab.AsDataView();
            odswiezlisty();
        }
        // lista object na lista double
        private List<Double> castToDouble(List<object> toCast)
        {
            List<Double> temp = new List<Double>();

            foreach (object o in toCast)
                temp.Add((Double)o);
            return temp;
        }

        private List<String> castToString(List<object> toCast)
        {
            List<String> temp = new List<String>();

            foreach (object o in toCast)
                    temp.Add((string)o);

            return temp;
        }

        // Dyskretyzacja n klas na n liczbowych
        private void zm_klas_Click(object sender, RoutedEventArgs e)
        {
            //object[] wyst = zliczWystapienia();
            List<object> wyst = zliczWystapienia();

            daneTab.Columns.Add("DYSNUM_K" + (wybor.SelectedIndex+1), typeof(int));
            int i=0;
            foreach (DataRow row in daneTab.Rows)
            {
                i = wyst.FindIndex(x => x.Equals(row[wybor.SelectedIndex]));
                row[row.ItemArray.Length - 1] = i + 1;
            }
            blok.ItemsSource = daneTab.AsDataView();
            odswiezlisty();
        }

        // Zlicza ile jest klas i wyrzuca je w liście w kolejności wystąpienia
        private List<object> zliczWystapienia()
        {
            List<object> wyst = new List<object>();

            foreach (DataRow row in daneTab.Rows)
            {
                if (wyst.Contains(row[wybor.SelectedIndex]))
                    continue;
                else
                    wyst.Add(row[wybor.SelectedIndex]);
            }
            return wyst;
        }

        //Zlicza wystąpienie klas do funkcji rysującej
        private List<object> zliczWystapieniaDoSlownika()
        {
            List<object> wyst = new List<object>();

            foreach (DataRow row in daneTab.Rows)
            {
                if (wyst.Contains(row[wybor_chart_klasa.SelectedIndex]))
                    continue;
                else
                    wyst.Add(row[wybor_chart_klasa.SelectedIndex]);
            }
            return wyst;
        }

        private List<object> doListy()
        {
            List<object> temp = new List<object>();

            foreach (DataRow r in daneTab.Rows)
            {
                temp.Add(r[wybor.SelectedIndex]);
            }
            return temp;
        }

        //Dyskretyzacja n klas na k klas
        private void wyb_nom_Click(object sender, RoutedEventArgs e)
        {
            List<object> wyst = zliczWystapienia();

            List<object> list = doListy();


            int ileKlas = Int32.Parse(liczKlas.Text);
            
            //var g = list.GroupBy(z => z);
            var licz = 
                from k in list 
                group k by k into g 
                select new { klucz = g.Key, count= g.Count() };

            var sorted =
                from k in licz
                orderby k.count descending
                select k;

            //licz.OrderByDescending(k => k.count);

            daneTab.Columns.Add("DYSNUM_K" + (wybor.SelectedIndex +1) + "_N" + ileKlas, typeof(int));
            int i = 0;
            foreach ( var s in sorted)
            {
                i++;
                if (i > ileKlas)
                    i = ileKlas;
                foreach (DataRow row in daneTab.Rows)
                {
                    //i = wyst.FindIndex(x => x.Equals(row[wybor.SelectedIndex]));
                    if ( row[wybor.SelectedIndex].Equals(s.klucz))
                        row[row.ItemArray.Length - 1] = i;
                }
            } 
            
            blok.ItemsSource = daneTab.AsDataView();
            odswiezlisty();
        }

        //Rysowanie
        private void rys_wykres_Click(object sender, RoutedEventArgs e)
        {
            List<object> klasy = zliczWystapieniaDoSlownika();
            Dictionary<double, double> slownik;
            WindowChart oknowykres = new WindowChart();

            foreach (object o in klasy)
            {
                slownik = new Dictionary<double, double>();
                foreach (DataRow r in daneTab.Rows)
                {
                    if (!(r[wybor_chart_klasa.SelectedIndex].Equals(o)))
                       continue;

                    if (!slownik.ContainsKey(Double.Parse(r[wybor_chart1.SelectedIndex].ToString())))
                        slownik.Add(Double.Parse(r[wybor_chart1.SelectedIndex].ToString()), Double.Parse(r[wybor_chart2.SelectedIndex].ToString()));
                }
                oknowykres.addSeries(slownik);
            }
            oknowykres.Show();
        }

        private void norm_Click(object sender, RoutedEventArgs e)
        {
            double odchylenie = Math.Sqrt(wariancja());
            double avg = srednia();

            daneTab.Columns.Add("NORM_K" + wybor.SelectedIndex, typeof(Double));

            foreach (DataRow r in daneTab.Rows)
            {
                r[r.ItemArray.Length -1] = (((double)r[wybor.SelectedIndex] - avg) / odchylenie);
            }
            blok.ItemsSource = daneTab.AsDataView();
            odswiezlisty();
        }

        private void cos_Click(object sender, RoutedEventArgs e)
        {
            double newMin = Double.Parse(norm_przed.Text);
            double newMax = Double.Parse(norm_przed2.Text);

            //double min =  
            //double max =

            daneTab.Columns.Add("NORM_K" + (wybor.SelectedIndex +1) + "MIN" + newMin + "MAX" + newMax, typeof(Double));

            foreach (DataRow r in daneTab.Rows)
            {
               // r[r.ItemArray.Count-1] = 
            }
        }        
    }
}