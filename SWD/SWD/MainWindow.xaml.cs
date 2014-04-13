using Microsoft.Win32;
using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;



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
                    /*long liczbaWierszy = File.ReadAllLines(filename).LongLength;

                    DataSet wynik = new DataSet();
                    string[] textData = System.IO.File.ReadAllLines(filename);
                    string[] naglowki = textData[0].Split(' ', ',');

                    DataTable dataTable1 = new DataTable();
                    foreach (string header in naglowki)
                        dataTable1.Columns.Add(header, typeof(string), null);
                    for (int i = 1; i < liczbaWierszy; i++)
                        dataTable1.Rows.Add(textData[i].Split(','));
                    blok.DataContext = dataTable1;*/

                    string[] textData = System.IO.File.ReadAllLines(filename);
                    string[] naglowki = textData[0].Split(' ', ',');
                    DataTable dt = new DataTable();
                    foreach (string c in naglowki)
                    {
                        dt.Columns.Add(c);
                    }
                    string newline;
                    while ((newline = ) != null)
                    {
                        DataRow dr = dt.NewRow();
                        string[] values = newline.Split(' ');
                        for (int i = 0; i < values.Length; i++)
                        {
                            dr[i] = values[i];
                        }
                        dt.Rows.Add(dr);
                    }
                    file.Close();
                    blok.ItemSource = dt;



                    //long liczbaWierszy = File.ReadAllLines(filename).LongLength;
                    //long liczbaKolumn = File.ReadAllLines(filename)[0].Split(' ', ',').LongLength;

                    //object[][] tablica = new object[liczbaKolumn][];

                    //foreach(string k in File.ReadAllLines(filename))
                    //{
                    //    foreach(string l in k.Split(' ', ','))
                    //    {

                    //    }
                    //}

                    /*foreach (string k in File.ReadAllLines(filename))
                    {
                        int j = 0;
                        foreach (string l in k.Split(new char[] { ' ', ',' }))
                        {
                            if(listaKolumn.Count < k.Split(new char[] { ' ', ',' }).Length)
                            {
                                listaKolumn.Add(new List<object>());
                            }
                            double temp;
                            if (Double.TryParse(l, out temp))
                                listaKolumn[j].Add(temp);
                            else
                                listaKolumn[j].Add(l);
                            blok.Text += listaKolumn[j][listaKolumn[j].Count-1]+ " ";
                            j++;                            
                        }
                        blok.Text += "\n";
                    }*/

                    //CollectionViewSource items;
                    //items = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
                    //items.Source = listaKolumn;

                    //blok.ItemsSource = listaKolumn;

                    //for (int i = 0; i < listaKolumn.Count; i++)
                      //  wybor.Items.Add(new ItemObject("kolumna" + i, i));

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            

        }

        private void oblicz_Click(object sender, RoutedEventArgs e)
        {
            Double cnt=0;
            for(int i=0; i < listaKolumn[0].Count; i++)
            {
                cnt += (float)listaKolumn[((int)(wybor.SelectedValue))][i];
                //wybor.Sel
            }

            text_średnia.Text = (cnt / (float)listaKolumn[((int)(wybor.SelectedValue))].Count) + "";
        }
    }
}
