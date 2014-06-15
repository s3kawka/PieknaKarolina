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
using Accord.Math;
using Accord.Statistics;

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
            wybor_grupowanie.Items.Add(new ItemObject("Euklides", 0));
            wybor_grupowanie.Items.Add(new ItemObject("Manhattan", 0));
            wybor_grupowanie.Items.Add(new ItemObject("Lnieskonczonosc", 0));
            wybor_grupowanie.Items.Add(new ItemObject("Mahalanobis", 0));
            wybor_grupowanie.SelectedIndex = 0;
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
            naglowki = null;
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog(); 
                             
                if (result == true)
                {
                    Aktywuj();
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
                        // foreach dla każdego stringa wydzielonego z jednej przeczytaniej linii, stringa dzieli spacja, tabulator lub przecinek
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
                    wybor_chart_klasa.Items.Clear();

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
                    wybor_chart_klasa.SelectedIndex = 0;
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
                text_mediana.Text = "";
                text_minmax1.Text = "";
                text_minmax2.Text ="";
                text_kw1.Text = "";
                text_kw2.Text = "";
                text_p1.Text = "";
                text_p2.Text = "";
                text_p3.Text = "";
                text_p4.Text = "";
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
            if (przedzial.Text != "")
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
                    if (i != 0)
                        przedzialCur[0] += dlugosc;
                    przedzialCur[1] += dlugosc;
                    foreach (DataRow row in daneTab.Rows)
                    {
                        if ((double)row[wybor.SelectedIndex] >= przedzialCur[0] && (double)row[wybor.SelectedIndex] < przedzialCur[1])
                        {
                            row[row.ItemArray.Length - 1] = i + 1;
                        }
                        if (i == liczPrzedz - 1 && (double)row[wybor.SelectedIndex] == minMax[1])
                            row[row.ItemArray.Length - 1] = i + 1;
                    }

                }
                blok.ItemsSource = daneTab.AsDataView();
                odswiezlisty();
            }
            else
            {
                MessageBox.Show("wpisz wartosc");
            }
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

            if (przedzial.Text != "")
            {
                int ileKlas = Int32.Parse(przedzial.Text);

                //var g = list.GroupBy(z => z);
                var licz =
                    from k in list
                    group k by k into g
                    select new { klucz = g.Key, count = g.Count() };

                var sorted =
                    from k in licz
                    orderby k.count descending
                    select k;

                //licz.OrderByDescending(k => k.count);

                daneTab.Columns.Add("DYSNUM_K" + (wybor.SelectedIndex + 1) + "_N" + ileKlas, typeof(int));
                int i = 0;
                foreach (var s in sorted)
                {
                    i++;
                    if (i > ileKlas)
                        i = ileKlas;
                    foreach (DataRow row in daneTab.Rows)
                    {
                        //i = wyst.FindIndex(x => x.Equals(row[wybor.SelectedIndex]));
                        if (row[wybor.SelectedIndex].Equals(s.klucz))
                            row[row.ItemArray.Length - 1] = i;
                    }
                }

                blok.ItemsSource = daneTab.AsDataView();
                odswiezlisty();
            }
            else
            {
                MessageBox.Show("Wpisz wartość");
            }
            
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

            try
            {
                daneTab.Columns.Add("NORM_K" + wybor.SelectedIndex, typeof(Double));

                foreach (DataRow r in daneTab.Rows)
                {
                    r[r.ItemArray.Length - 1] = (((double)r[wybor.SelectedIndex] - avg) / odchylenie);
                }
                blok.ItemsSource = daneTab.AsDataView();
                odswiezlisty();
            }
            catch (DuplicateNameException ex)
            {
                MessageBox.Show("Już wykonałeś tą operację, dane w kolumnie " + "NORM_K" + wybor.SelectedIndex);
            }
        }

        private void cos_Click(object sender, RoutedEventArgs e)
        {
            if (norm_przed.Text != "" && norm_przed2.Text != "")
            {
                double newMin = Double.Parse(norm_przed.Text);
                double newMax = Double.Parse(norm_przed2.Text);

                double min = minmax()[0];
                double max = minmax()[1];

                daneTab.Columns.Add("NORM_K" + (wybor.SelectedIndex + 1) + "MIN" + newMin + "MAX" + newMax, typeof(Double));

                foreach (DataRow r in daneTab.Rows)
                {
                    r[r.ItemArray.Count() - 1] = ((((double)r[wybor.SelectedIndex] - min) / (max - min)) * (newMax - newMin)) + newMin;
                }
                blok.ItemsSource = daneTab.AsDataView();
                odswiezlisty();
            }
            else
            {
                MessageBox.Show("Wpisz wartość");
            }

            
        }

        private List<List<Double>> przepiszListe()
        {
            List<List<Double>> list = new List<List<Double>>();

            foreach (DataRow r in daneTab.Rows)
            {
                List<double> temp = new List<Double>();
                foreach (object d in r.ItemArray)
	            {
                    double outx;
                    if(Double.TryParse(d.ToString(), out outx))
		                temp.Add(outx);
	            }
                //temp.RemoveAt(temp.Count-1);
                list.Add(temp);
            }
            return list;		 
	    }

        private double[][] oblicz_odleglosci_euk()
        {
            int i,j;
            List<List<Double>> list = przepiszListe();

            //double[][] odleglosc = new double[daneTab.Rows.Count][];
            List<KeyValuePair<String, double>> odleglosc = new List<KeyValuePair<string,double>>();
            int[] knn = new int[list.Count - 1];

           // for (int x = 0; x < daneTab.Rows.Count; x++)
                //odleglosc[x] = new double[daneTab.Rows.Count];

            for (i = 0; i < list.Count; i++)
            {
                for (j = 0; j < list.Count; j++)
                {
                    if (i == j)
                        continue;
                    double dist = 0;
                    dist = Distance.Euclidean(list[i].ToArray(), list[j].ToArray());
                    odleglosc.Add(new KeyValuePair<string,double>(daneTab.Rows[j].ItemArray[daneTab.Columns.Count-1].ToString(),dist));
                }
                odleglosc.Sort(ckv);
                string curClass = daneTab.Rows[i].ItemArray[daneTab.Columns.Count-1].ToString();
                int dobre=0;
                for (int h = 1; h < list.Count - 1; h++)
                {
                    //foreach (KeyValuePair<string, double> kv in odleglosc.Take(h))
                    //{
                    //    if (kv.Key == curClass)
                    //        dobre++;
                    //}
                    
                    var licz = 
                        from o in odleglosc.Take(h)
                        group o by o.Key into g 
                        select new { a = g.Key, b = g.Count() };

                    licz = licz.OrderByDescending(v => v.b);

                    if (licz.ToList()[0].a == curClass)
                    {
                      knn[h]++;
                    }
                }
                odleglosc = new List<KeyValuePair<string, double>>();
            }
            WindowChart x = new WindowChart();
            Dictionary<double,double> dict = new Dictionary<double,double>();
            for(int u = 1; u<knn.Length; u++)
            {
                dict.Add(u,knn[u]);
            }

            x.addSeries(dict);
            x.Show();

            return null;
        }

        private double[][] oblicz_odleglosci_man()
        {
            int i, j;
            List<List<Double>> list = przepiszListe();

            //double[][] odleglosc = new double[daneTab.Rows.Count][];
            List<KeyValuePair<String, double>> odleglosc = new List<KeyValuePair<string, double>>();
            int[] knn = new int[list.Count - 1];

            // for (int x = 0; x < daneTab.Rows.Count; x++)
            //odleglosc[x] = new double[daneTab.Rows.Count];

            for (i = 0; i < list.Count; i++)
            {
                for (j = 0; j < list.Count; j++)
                {
                    if (i == j)
                        continue;
                    double dist = 0;
                    dist = Distance.Manhattan(list[i].ToArray(), list[j].ToArray());
                    odleglosc.Add(new KeyValuePair<string, double>(daneTab.Rows[j].ItemArray[daneTab.Columns.Count - 1].ToString(), dist));
                }
                odleglosc.Sort(ckv);
                string curClass = daneTab.Rows[i].ItemArray[daneTab.Columns.Count - 1].ToString();
                int dobre = 0;
                for (int h = 1; h < list.Count - 1; h++)
                {
                    //foreach (KeyValuePair<string, double> kv in odleglosc.Take(h))
                    //{
                    //    if (kv.Key == curClass)
                    //        dobre++;
                    //}

                    var licz =
                        from o in odleglosc.Take(h)
                        group o by o.Key into g
                        select new { a = g.Key, b = g.Count() };

                    licz = licz.OrderByDescending(v => v.b);

                    if (licz.ToList()[0].a == curClass)
                    {
                        knn[h]++;
                    }
                }
                odleglosc = new List<KeyValuePair<string, double>>();
            }
            WindowChart x = new WindowChart();
            Dictionary<double, double> dict = new Dictionary<double, double>();
            for (int u = 1; u < knn.Length; u++)
            {
                dict.Add(u, knn[u]);
            }

            x.addSeries(dict);
            x.Show();

            return null;
        }

        private double[][] oblicz_odleglosci_lnie()
        {
            int i, j;
            List<List<Double>> list = przepiszListe();

            //double[][] odleglosc = new double[daneTab.Rows.Count][];
            List<KeyValuePair<String, double>> odleglosc = new List<KeyValuePair<string, double>>();
            int[] knn = new int[list.Count - 1];

            // for (int x = 0; x < daneTab.Rows.Count; x++)
            //odleglosc[x] = new double[daneTab.Rows.Count];

            for (i = 0; i < list.Count; i++)
            {
                for (j = 0; j < list.Count; j++)
                {
                    if (i == j)
                        continue;
                    double dist = 0;
                    dist = Distance.Chebyshev(list[i].ToArray(), list[j].ToArray());//(list[i], list[j]);
                    odleglosc.Add(new KeyValuePair<string, double>(daneTab.Rows[j].ItemArray[daneTab.Columns.Count - 1].ToString(), dist));
                }
                odleglosc.Sort(ckv);
                string curClass = daneTab.Rows[i].ItemArray[daneTab.Columns.Count - 1].ToString();
                int dobre = 0;
                for (int h = 1; h < list.Count - 1; h++)
                {
                    //foreach (KeyValuePair<string, double> kv in odleglosc.Take(h))
                    //{
                    //    if (kv.Key == curClass)
                    //        dobre++;
                    //}

                    var licz =
                        from o in odleglosc.Take(h)
                        group o by o.Key into g
                        select new { a = g.Key, b = g.Count() };

                    licz = licz.OrderByDescending(v => v.b);

                    if (licz.ToList()[0].a == curClass)
                    {
                        knn[h]++;
                    }
                }
                odleglosc = new List<KeyValuePair<string, double>>();
            }
            WindowChart x = new WindowChart();
            Dictionary<double, double> dict = new Dictionary<double, double>();
            for (int u = 1; u < knn.Length; u++)
            {
                dict.Add(u, knn[u]);
            }

            x.addSeries(dict);
            x.Show();

            return null;
        }

        private double[][] oblicz_odleglosci_mahal()
        {
            int i, j, cnt;
            cnt = 0;
            List<List<Double>> list = przepiszListe();

            //double[][] odleglosc = new double[daneTab.Rows.Count][];
            List<KeyValuePair<String, double>> odleglosc = new List<KeyValuePair<string, double>>();
            int[] knn = new int[list.Count - 1];

            // for (int x = 0; x < daneTab.Rows.Count; x++)
            //odleglosc[x] = new double[daneTab.Rows.Count];

            for (i = 0; i < list.Count; i++)
            {
                for (j = 0; j < list.Count; j++)
                {
                    if (i == j)
                        continue;
                    double dist = 0;
                    cnt++;
                    double[,] matrix = new double[2, list[j].Count];
                    for(int r=0; r<list[j].Count;r++)
                    {
                        matrix[0,r] = list[i][r];
                        matrix[1,r] = list[j][r];
                    }

                    double[,] cov = Accord.Statistics.Tools.Covariance(matrix);
                    double[,] invertedcov = Accord.Math.Matrix.Inverse(cov);
                    dist = Distance.Mahalanobis(list[i].ToArray(), list[j].ToArray(), invertedcov);//(list[i], list[j]);
                    odleglosc.Add(new KeyValuePair<string, double>(daneTab.Rows[j].ItemArray[daneTab.Columns.Count - 1].ToString(), dist));
                }
                odleglosc.Sort(ckv);
                string curClass = daneTab.Rows[i].ItemArray[daneTab.Columns.Count - 1].ToString();
                int dobre = 0;
                for (int h = 1; h < list.Count - 1; h++)
                {
                    cnt++;
                    //foreach (KeyValuePair<string, double> kv in odleglosc.Take(h))
                    //{
                    //    if (kv.Key == curClass)
                    //        dobre++;
                    //}

                    var licz =
                        from o in odleglosc.Take(h)
                        group o by o.Key into g
                        select new { a = g.Key, b = g.Count() };

                    licz = licz.OrderByDescending(v => v.b);

                    if (licz.ToList()[0].a == curClass)
                    {
                        knn[h]++;
                    }
                }
                odleglosc = new List<KeyValuePair<string, double>>();
            }
            WindowChart x = new WindowChart();
            Dictionary<double, double> dict = new Dictionary<double, double>();
            for (int u = 1; u < knn.Length; u++)
            {
                dict.Add(u, knn[u]);
            }

            x.addSeries(dict);
            x.Show();

            return null;
        }

        private void knn_Click(object sender, RoutedEventArgs e)
        {
            oblicz_odleglosci_euk();
        }

        static int ckv(KeyValuePair<string, double> a, KeyValuePair<string, double> b)
        {
            return a.Value.CompareTo(b.Value);
        }

        private void knn_man_Click(object sender, RoutedEventArgs e)
        {
            oblicz_odleglosci_man();
        }

        private void knn_lnie_Click(object sender, RoutedEventArgs e)
        {
            oblicz_odleglosci_lnie();
        }

        double lnie(List<double> p1, List<double> p2)
        {
            double temp = Double.MinValue;
            for(int i = 0;i<p1.Count;i++)
            {
                if (temp < p1[i] - p2[i])
                {
                    temp = p1[i] - p2[i];
                }
            }

            return temp;
        }

        private void knn_maha_Click(object sender, RoutedEventArgs e)
        {
            oblicz_odleglosci_mahal();
        }

        private void zapisz_Click(object sender, RoutedEventArgs e)
        {
            
            SaveFileDialog dialog = new SaveFileDialog();
            Nullable<bool> result = dialog.ShowDialog();
            dialog.Filter = "Text Files|*.txt";

            if (result == true)
            {
                using ( Stream s = File.Open(dialog.FileName+".txt", FileMode.CreateNew))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    foreach (DataRow row in daneTab.Rows)
                    {
                        foreach (DataColumn col in daneTab.Columns)
                        {
                            sw.Write(row[col].ToString() + " ");

                        }
                        sw.WriteLine();
                    }
                }
            }
        }
        public void Aktywuj() 
        {
            norm.IsEnabled = true;
            dyskr.IsEnabled = true;
            zm_klas.IsEnabled = true;
            wyb_nom.IsEnabled = true;
            cos.IsEnabled = true;
            rys_wykres.IsEnabled = true;
            knn.IsEnabled = true;
            knn_lnies.IsEnabled = true;
            knn_man.IsEnabled = true;
            ksred_but.IsEnabled = true;
            kmedi_but.IsEnabled = true;
        }

        private void ksred_but_Click(object sender, RoutedEventArgs e)
        {
            //1 wybieram k punktów
            //2 przydzielam klasy euk/mah/lnies
            //3 licze srednia/mediane po wszystkich punktach
            //4 ustawiam nowe punkty
            //go to 2
        }

        private void kmedi_but_Click(object sender, RoutedEventArgs e)
        {



        }
        
    }
}