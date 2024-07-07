using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using System.Xml.Schema;


namespace Dzietność
{
    public partial class Form1 : Form
    {
       
        string dolnoslaskie = $@"{AppDomain.CurrentDomain.BaseDirectory}\DaneDzietnosc\dzietnoscDolnoslaskie.csv";
        string polska = $@"{AppDomain.CurrentDomain.BaseDirectory}\DaneDzietnosc\dzietnoscPolska.csv";
        static string xmlpathd = $@"{AppDomain.CurrentDomain.BaseDirectory}\DaneDzietnosc\dzietnosc.xml";
        static string xmlpathp = $@"{AppDomain.CurrentDomain.BaseDirectory}\Danepkb\pkb.xml";
        string targetYear = "rok_2022";
        
        

        


        public Form1(string nazwa)
        {


            
            InitializeComponent();
            label5.Text = "Użytkownik: "+nazwa;
            chart1.Series.Clear();
            for (int i = 2002; i <= 2022 ; i++)
            {
                string a = i.ToString();
                comboBox2.Items.Add(a);
            }
            checkedListBox1.BringToFront();

        }
       
        public void generowanie(string kategoria, Chart wykres)
        {
            string xmlpath="";
            wykres.Visible = true;
            if (kategoria == "pkb")
            {
                xmlpath = xmlpathp;
            }
            else if (kategoria == "dzietnosc")
            {
                xmlpath = xmlpathd;
            }

            XDocument doc = XDocument.Load(xmlpath);
            if (comboBox2.SelectedItem == null || comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Wybierz Przedział lat");
            }
            else
            { 
                wykres.Series.Clear();
                foreach (var item in checkedListBox1.CheckedItems)
                {
                    string targetRegion = item.ToString().ToUpper();
                    string napis = kategoria + " " + item.ToString();
                    string a = $@"{AppDomain.CurrentDomain.BaseDirectory}\Dane{kategoria}\{kategoria}{item}.csv";
                    List<string> stringList = new List<string>();
                    //string[] tab = new string [21];
                    for (int i = 2002; i <2023; i++)
                    {
                        string temp = "rok_" + i.ToString();
                        stringList.Add(temp);
                    }
                    string[] tab = stringList.ToArray();
                    
                    var record = doc.Descendants("record")
                        .Where(r => r.Element("Nazwa").Value == targetRegion)
                        .FirstOrDefault();
                    
                        /*var yearElement = record.Descendants(targetYear).FirstOrDefault();
                        if (yearElement != null)
                        {
                            MessageBox.Show($"Dzietność w {targetYear} dla regionu {targetRegion}: {yearElement.Value}");
                        }
                        else
                        {
                            MessageBox.Show($"Nie znaleziono danych dla roku {targetYear}.");
                        }*/
                    

                    /*
                    List<string> lines = File.ReadAllLines(a).ToList();
                    foreach (string line in lines)
                    {
                        tab = line.Split(';');

                    }
                    */
                    
                    double[] dane = new double[tab.Length];
                if (record != null)
                {
                    for (int i = 0; i < tab.Length; i++)
                    {
                        var yearElement = record.Descendants(tab[i]).FirstOrDefault();
                        dane[i] = double.Parse(yearElement.Value,CultureInfo.InvariantCulture);
                    }
                }
                    wykres.Series.Add(napis);
                    if (comboBox1.SelectedItem == "Linie")
                    {
                        wykres.Series[napis].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                    }
                    else if (comboBox1.SelectedItem == "Kolumny")
                    {
                        wykres.Series[napis].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    }

                    int zrok1 = int.Parse(comboBox2.SelectedItem.ToString()) - 2002;

                    int zrok2 = int.Parse(comboBox3.SelectedItem.ToString()) - 2002;
                    for (int i = 0 + zrok1; i <= zrok2; i++)
                    {
                        
                        wykres.Series[napis].Points.AddXY(i + 2002, dane[i]);
                    }
                    
                }
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            chart2.Visible = false;
            generowanie("dzietnosc", chart1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.SelectedItem = null;
            comboBox3.Items.Clear();
            
            
            string a = comboBox2.SelectedItem.ToString();
            int l = int.Parse(a);
            
            for(int i = l + 1; i <= 2022; i++)
            {
                comboBox3.Items.Add(i.ToString());
            }
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Visible = false;
            generowanie("pkb", chart2);
            
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }
        private void chart2_Click_2(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string configFilePath = "config.json";

                if (File.Exists(configFilePath))
                {
                    File.Delete(configFilePath);
                    MessageBox.Show("Plik config.json został usunięty.");
                }
                else
                {
                    MessageBox.Show("Plik config.json nie istnieje.");
                }

                Application.Exit(); // Ukryj aktualne okno logowania
                
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas usuwania pliku config.json: {ex.Message}");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string kategoria ="";
            if (chart1.Visible)
            {
                kategoria = "Dzietność";
            }
            else if (chart2.Visible) 
            {
                kategoria = "PKB";
            }
            // Pobierz dane z wykresu
            var chart = chart1; // lub chart2, w zależności od wybranego wykresu
            Dictionary<string, List<DataPoint>> dataPointsBySeries = new Dictionary<string, List<DataPoint>>();

            foreach (var series in chart.Series)
            {
                dataPointsBySeries[series.Name] = new List<DataPoint>();
                foreach (var point in series.Points)
                {
                    dataPointsBySeries[series.Name].Add(point);
                }
            }

            // Wyeksportuj dane do pliku XML
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog.Title = "Zapisz dane wykresu";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExportDataToXml(dataPointsBySeries, saveFileDialog.FileName, kategoria);
                MessageBox.Show("Dane zostały wyeksportowane pomyślnie.", "Eksport danych", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void ExportDataToXml(Dictionary<string, List<DataPoint>> dataPointsBySeries, string filePath, string kategoria)
        {
            var doc = new XDocument(
                new XElement("Chart",
                    dataPointsBySeries.Select(series => new XElement("Series",
                        new XAttribute("Name", series.Key),
                        series.Value.Select(point => new XElement("DataPoint",
                            new XElement("Rok", point.XValue),
                            new XElement(kategoria, point.YValues.FirstOrDefault())
                        ))
                    ))
                )
            );

            doc.Save(filePath);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            openFileDialog.Title = "Wczytaj dane wykresu";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    bool dataLoaded = LoadDataFromXml(filePath);

                    if (dataLoaded)
                    {
                        MessageBox.Show("Dane zostały wczytane pomyślnie.", "Wczytywanie danych", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nie wczytano żadnych danych do wykresu.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas wczytywania pliku XML: {ex.Message}");
                }
            }
            chart2.SendToBack();
        }
        private bool LoadDataFromXml(string filePath)
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania pliku XML: {ex.Message}");
                return false;
            }

            // Clear existing data in the chart
            chart1.Series.Clear();

            bool dataLoaded = false;

            var seriesElements = doc.Descendants("Series");
            if (!seriesElements.Any())
            {
                MessageBox.Show("Brak elementów Series w pliku XML.");
                return false;
            }

            foreach (var seriesElement in seriesElements)
            {
                string seriesName = seriesElement.Attribute("Name").Value;
                var series = new Series(seriesName);
                MessageBox.Show($"Dodawanie serii: {seriesName}");

                foreach (var dataPointElement in seriesElement.Descendants("DataPoint"))
                {
                    try
                    {
                        double xValue = double.Parse(dataPointElement.Element("Rok").Value, CultureInfo.InvariantCulture);
                        double yValue = double.Parse(dataPointElement.Element("Dzietność")?.Value ?? dataPointElement.Element("PKB").Value, CultureInfo.InvariantCulture);

                        series.Points.AddXY(xValue, yValue);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Błąd podczas parsowania DataPoint: {ex.Message}");
                        continue;
                    }
                }

                chart1.Series.Add(series);
                dataLoaded = true;
            }

            chart1.Visible = chart1.Series.Count > 0;

            return dataLoaded;
        }
    }
}
