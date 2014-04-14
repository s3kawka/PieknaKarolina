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
                        int j = 0;
                        // foreach dla każdego stringa wydzielonego z jednej przeczytaniej linii, stringa dzieli spacja lub przecinek
                        foreach (string l in k.Split(new char[] { ' ', ',', '\t' }))
                        {
                            // dodanie list reprezentujących kolumny, jednorazowo przy pierwszej iteracji
                            if(listaKolumn.Count < k.Split(new char[] { ' ', ',', '\t' }).Length)
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

                    DataTable dane = new DataTable();

                    // Stworzenie kolumn w DataTable
                    for (int i = 0; i < listaKolumn.Count; i++)
                    {
                        double temp;
                        // Rzutowanie jak przy tworzeniu list, jak liczba to typu double jak nie to String
                        if (Double.TryParse(listaKolumn[i][0].ToString(), out temp))
                            dane.Columns.Add("Column" + (i + 1), typeof(Double));
                        else
                            dane.Columns.Add("Column" + (i + 1), typeof(String));
                    }
                    // Wpisanie danych do DataTable pierwszy for po wierszach, wewnętrzny po kolumnach
                    for (int i = 0; i < listaKolumn[0].Count; i++)
                    {
                        DataRow row = dane.Rows.Add();
                        for (int j = 0; j < listaKolumn.Count; j++)
                            row[j] = listaKolumn[j][i];
                    }
                    
                    // Zbindowanie danych z DataTable do GridView
                    blok.ItemsSource = dane.AsDataView();


                    // Wpisanie kolum do listy rozwijanej
                    for (int i = 0; i < listaKolumn.Count; i++)
                        wybor.Items.Add(new ItemObject("kolumna" + (i+1), i));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            

        }

        private void wybor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Double cnt = 0;
            for (int i = 0; i < listaKolumn[0].Count; i++)
            {
                cnt += (Double)listaKolumn[wybor.SelectedIndex][i];
            }

            text_średnia.Text = (cnt / listaKolumn[wybor.SelectedIndex].Count) + "";
            text_mediana.Text = mediana() + "";

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
    }
}