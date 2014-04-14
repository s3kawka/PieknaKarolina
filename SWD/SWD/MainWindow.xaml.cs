using Microsoft.Win32;
using System;
using System.Collections;
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
                            if (Double.TryParse(l, NumberStyles.Any, CultureInfo.InvariantCulture, out temp))
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
                                daneTab.Columns.Add("Column" + (i + 1), typeof(Double));
                            else
                                daneTab.Columns.Add("Column" + (i + 1), typeof(String));
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

                    if (naglowki != null)
                    {
                        int i = 0;
                        foreach (string l in naglowki.Split(new char[] { ' ', '\t', ';' }))
                        {

                            wybor.Items.Add(new ItemObject(l, i));
                            i++;
                        }
                    }
                    else
                        for (int i = 0; i < listaKolumn.Count; i++)
                            wybor.Items.Add(new ItemObject("kolumna" + (i + 1), i));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Niepoprawny plik");
                throw;
            }
        }

        private void wybor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                text_średnia.Text = srednia() + "";
                text_mediana.Text = mediana() + "";
                String[] mm1 = minmax();
                text_minmax1.Text = mm1[0];
                text_minmax2.Text = mm1[1];
                String[] q1 = kwartyle();
                text_kw1.Text = q1[0];
                text_kw2.Text = q1[1];
                String[] p1 = percentyle();
                text_p1.Text = p1[0];
                text_p2.Text = p1[1];
                text_p3.Text = p1[2];
                text_p4.Text = p1[3];
            }
            catch (InvalidCastException ex)
            {
                text_średnia.Text = "Nie liczbowa kolumna";
            }
        }

        public Double srednia()
        {
            Double cnt = 0;
            for (int i = 0; i < listaKolumn[0].Count; i++)
            {
                cnt += (Double)listaKolumn[wybor.SelectedIndex][i];
            }

            cnt = (cnt / listaKolumn[wybor.SelectedIndex].Count);
            return cnt;
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

        public string[] minmax()
        {
            string[] mm = new string[2];
            int liczba_wierszy = listaKolumn[wybor.SelectedIndex].Count;

            listaKolumn[wybor.SelectedIndex].Sort();

            mm[0] = listaKolumn[wybor.SelectedIndex][0].ToString();
            mm[1] = listaKolumn[wybor.SelectedIndex][liczba_wierszy - 1].ToString();

            return mm;

        }

        public string[] kwartyle()
        {

            string[] q = new string[2];
            int liczba_wierszy = listaKolumn[wybor.SelectedIndex].Count;
            double a = (double)(liczba_wierszy / 4);

            listaKolumn[wybor.SelectedIndex].Sort();

            q[0] = listaKolumn[wybor.SelectedIndex][(int)a].ToString();
            q[1] = listaKolumn[wybor.SelectedIndex][(int)(a * 3)].ToString();

            return q;
        }

        public string[] percentyle()
        {

            string[] p = new string[4];
            double liczba_wierszy = listaKolumn[wybor.SelectedIndex].Count;
            double a = liczba_wierszy / (double)100;

            listaKolumn[wybor.SelectedIndex].Sort();

            p[0] = listaKolumn[wybor.SelectedIndex][(int)(5 * a)].ToString();
            p[1] = listaKolumn[wybor.SelectedIndex][(int)(10 * a)].ToString();
            p[2] = listaKolumn[wybor.SelectedIndex][(int)(90 * a)].ToString();
            p[3] = listaKolumn[wybor.SelectedIndex][(int)(95 * a)].ToString();

            return p;
        }
    }
}