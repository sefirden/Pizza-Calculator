using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Com.Syncfusion.Charts;
using Android.Graphics;
using Android.Content.PM;

namespace Pizza_Calculator
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "CompareActivity")]
    public class CompareActivity : Activity
    {
        //добавляем расчеты в клас

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Compare);

            string listAsString = Intent.GetStringExtra("saved_counter"); //эти две строки передают список пицц в это активити
            List<PizzaList> pizza = JsonConvert.DeserializeObject<List<PizzaList>>(listAsString);

            List<CompareList> compares = new List<CompareList>();
            int pizzanumber = 0;
            while (pizzanumber < pizza.Count())
            {
                compares.Add(new CompareList("Pizza #" + Convert.ToString(pizzanumber + 1), pizza[pizzanumber].GetArea(), pizza[pizzanumber].GetEdgeLength(), pizza[pizzanumber].PriceToArea(), pizza[pizzanumber].PriceToWeight(), 10));
                pizzanumber++;
            }

            //ниже дикий треш из 4-ч графиков. Принцип у всех одинаковый, меняется только название переменных.

            //------------------------------------------------------------------------первый график 
            SfChart chartArea = new SfChart(this);

            //Initializing primary axis
            CategoryAxis primaryAxisArea = new CategoryAxis();
            primaryAxisArea.Title.Text = "Номер пиццы в списке"; //подпись 
            chartArea.PrimaryAxis = primaryAxisArea;
            chartArea.PrimaryAxis.AutoScrollingDelta = 4; //на графике 4 максимум
            chartArea.PrimaryAxis.AutoScrollingMode = ChartAutoScrollingMode.Start; //стартуем с начала при пролистыавании
            chartArea.PrimaryAxis.Interval = 1;

            chartArea.TooltipCreated += ChartArea_TooltipCreated;


            //Initializing secondary Axis
            NumericalAxis secondaryAxisArea = new NumericalAxis();
            secondaryAxisArea.Title.Text = "Площадь, m²";
            chartArea.SecondaryAxis = secondaryAxisArea;

            //Initializing column series
            ColumnSeries seriesArea = new ColumnSeries();
            seriesArea.ItemsSource = compares;
            seriesArea.XBindingPath = "Name";
            seriesArea.YBindingPath = "Area";
            seriesArea.ColorModel.ColorPalette = ChartColorPalette.Metro;//цвет
            seriesArea.DataMarker.LabelStyle.LabelFormat = "#.#### m²";//формат чисел
            seriesArea.DataMarkerLabelCreated += SeriesArea_DataMarkerLabelCreated;
            
            seriesArea.DataPointSelectionEnabled = true;//выбрать столбик

            //ивент выбора столбика, когда столбюик выбран считаем по методу расчеты в % и показываем их, когда столбик не выбран показываем стадартные рассчеты
            int idxArea = 0;

            chartArea.SelectionChanged += Chart_SelectionChanged;

            void Chart_SelectionChanged(object sender, SfChart.SelectionChangedEventArgs e) 
            {
                
                if (e.P1.SelectedDataPointIndex > -1)
                {
                    int index = seriesArea.SelectedDataPointIndex;

                    while (idxArea < compares.Count())
                    {
                        if (compares[index].Area != 0)
                        {
                            compares[idxArea].InPercent = Math.Round((compares[idxArea].Area / compares[index].Area) * 100 - 100, 2);
                        }
                        
                        idxArea++;
                    }
                    seriesArea.YBindingPath = "InPercent";
                    seriesArea.DataMarker.LabelStyle.LabelFormat = "#.##'%'";//формат чисел
                    secondaryAxisArea.Title.Text = "Сравнение в %";
                    seriesArea.SelectedDataPointIndex = -1;
                }
                else
                {
                     idxArea = 0;
                     seriesArea.YBindingPath = "Area";
                     seriesArea.DataMarker.LabelStyle.LabelFormat = "#.#### m²";//формат чисел
                     secondaryAxisArea.Title.Text = "Площадь, m²";
                }
            }

            seriesArea.DataMarker.ShowLabel = true;
            seriesArea.Label = "Площадь пиццы, m². Больше - лучше";
            seriesArea.TooltipEnabled = true;
            chartArea.Title.Text = "Нажмите на столбец для сравнения в %";


            ChartZoomPanBehavior zoomArea = new ChartZoomPanBehavior();// скрол в сторону
            zoomArea.DoubleTapEnabled = false;
            zoomArea.ZoomingEnabled = false;
            zoomArea.ScrollingEnabled = true;
            chartArea.Behaviors.Add(zoomArea);

            chartArea.Series.Add(seriesArea);
            chartArea.Legend.Visibility = Visibility.Visible;
            FindViewById<LinearLayout>(Resource.Id.LayoutChartArea).AddView(chartArea);

            void SeriesArea_DataMarkerLabelCreated(object sender, ChartSeries.DataMarkerLabelCreatedEventArgs e)
            {
                var data = e.DataMarkerLabel.Data as CompareList;
                if (data != null && (chartArea.Series[0] as ColumnSeries).YBindingPath == "InPercent" && data.InPercent == 0)
                {
                    TextView textArea = new TextView(this) { Text = "0 %"};
                    textArea.SetTextColor(Color.Black);
                    textArea.Click += TextArea_Click;
                    e.DataMarkerLabel.View = textArea;
                }
            }

            void TextArea_Click(object sender, EventArgs e)
            {
                idxArea = 0;              
                seriesArea.YBindingPath = "Area";
                seriesArea.DataMarker.LabelStyle.LabelFormat = "#.#### m²";
                seriesArea.SelectedDataPointIndex = -1;
                secondaryAxisArea.Title.Text = "Площадь, m²";

                var textView = sender as TextView;
                textView.Text = string.Empty;
                textView = null;
            }


            //тултип не работает, что-то придумать
            void ChartArea_TooltipCreated(object sender, SfChart.TooltipCreatedEventArgs e)
            {
                var ser = e.P1.Series;
                var data = e.P1.Series.ItemsSource as List<PizzaList>;
                if (ser.SelectedDataPointIndex > -1)
                    e.P1.Label = "Quantity :" + data[ser.SelectedDataPointIndex].Quantity.ToString() + "\n Diameter :" + data[ser.SelectedDataPointIndex].diameter.ToString();
            }

            //-------------------------------------------------------------------------конец первого графика

            //------------------------------------------------------------------------второй график 

            SfChart chartEdgeLength = new SfChart(this);

            //Initializing primary axis
            CategoryAxis primaryAxisEdgeLength = new CategoryAxis();
            //primaryAxisEdgeLength.Title.Text = "Номер пиццы в списке"; //подпись 
            chartEdgeLength.PrimaryAxis = primaryAxisEdgeLength;
            chartEdgeLength.PrimaryAxis.AutoScrollingDelta = 4; //на графике 4 максимум
            chartEdgeLength.PrimaryAxis.AutoScrollingMode = ChartAutoScrollingMode.Start; //стартуем с начала при пролистыавании
            chartEdgeLength.PrimaryAxis.Interval = 1;


            chartEdgeLength.TooltipCreated += ChartEdgeLength_TooltipCreated;


            //Initializing secondary Axis
            NumericalAxis secondaryAxisEdgeLength = new NumericalAxis();
            secondaryAxisEdgeLength.Title.Text = "Длина борта, сm";
            chartEdgeLength.SecondaryAxis = secondaryAxisEdgeLength;

            //Initializing column series
            ColumnSeries seriesEdgeLength = new ColumnSeries();
            seriesEdgeLength.ItemsSource = compares;
            seriesEdgeLength.XBindingPath = "Name";
            seriesEdgeLength.YBindingPath = "EdgeLength";
            seriesEdgeLength.ColorModel.ColorPalette = ChartColorPalette.Metro;//цвет
            seriesEdgeLength.DataMarker.LabelStyle.LabelFormat = "#.#### сm";//формат чисел
            seriesEdgeLength.DataMarkerLabelCreated += SeriesEdgeLength_DataMarkerLabelCreated;

            seriesEdgeLength.DataPointSelectionEnabled = true;//выбрать столбик

            //ивент выбора столбика, когда столбюик выбран считаем по методу расчеты в % и показываем их, когда столбик не выбран показываем стадартные рассчеты
            int idxEdgeLength = 0;

            chartEdgeLength.SelectionChanged += Chart_SelectionChangedchartEdgeLength;

            void Chart_SelectionChangedchartEdgeLength(object sender, SfChart.SelectionChangedEventArgs e)
            {

                if (e.P1.SelectedDataPointIndex > -1)
                {
                    int index = seriesEdgeLength.SelectedDataPointIndex;

                    while (idxEdgeLength < compares.Count())
                    {
                        if (compares[index].EdgeLength != 0)
                        {
                            compares[idxEdgeLength].InPercent = Math.Round((compares[idxEdgeLength].EdgeLength / compares[index].EdgeLength) * 100 - 100, 2);
                        }

                        idxEdgeLength++;
                    }
                    seriesEdgeLength.YBindingPath = "InPercent";
                    seriesEdgeLength.DataMarker.LabelStyle.LabelFormat = "#.##'%'";//формат чисел
                    secondaryAxisEdgeLength.Title.Text = "Сравнение в %";
                    seriesEdgeLength.SelectedDataPointIndex = -1;
                }
                else
                {
                    idxEdgeLength = 0;
                    seriesEdgeLength.YBindingPath = "EdgeLength";
                    seriesEdgeLength.DataMarker.LabelStyle.LabelFormat = "#.#### сm";//формат чисел
                    secondaryAxisEdgeLength.Title.Text = "Длина борта, сm";
                }
            }

            seriesEdgeLength.DataMarker.ShowLabel = true;
            seriesEdgeLength.Label = "Длина борта, сm.";
            seriesEdgeLength.TooltipEnabled = true;
            //chartEdgeLength.Title.Text = "Нажмите на столбец для сравнения в %";


            ChartZoomPanBehavior zoomEdgeLength = new ChartZoomPanBehavior();// скрол в сторону
            zoomEdgeLength.DoubleTapEnabled = false;
            zoomEdgeLength.ZoomingEnabled = false;
            zoomEdgeLength.ScrollingEnabled = true;
            chartEdgeLength.Behaviors.Add(zoomEdgeLength);

            chartEdgeLength.Series.Add(seriesEdgeLength);
            chartEdgeLength.Legend.Visibility = Visibility.Visible;
            FindViewById<LinearLayout>(Resource.Id.LayoutChartEdgeLength).AddView(chartEdgeLength);

            void SeriesEdgeLength_DataMarkerLabelCreated(object sender, ChartSeries.DataMarkerLabelCreatedEventArgs e)
            {
                var data = e.DataMarkerLabel.Data as CompareList;
                if (data != null && (chartEdgeLength.Series[0] as ColumnSeries).YBindingPath == "InPercent" && data.InPercent == 0)
                {
                    TextView textEdgeLength = new TextView(this) { Text = "0 %" };
                    textEdgeLength.SetTextColor(Color.Black);
                    textEdgeLength.Click += TextEdgeLength_Click;
                    e.DataMarkerLabel.View = textEdgeLength;
                }
            }

            void TextEdgeLength_Click(object sender, EventArgs e)
            {
                idxEdgeLength = 0;
                seriesEdgeLength.YBindingPath = "EdgeLength";
                seriesEdgeLength.DataMarker.LabelStyle.LabelFormat = "#.#### cm";
                seriesEdgeLength.SelectedDataPointIndex = -1;
                secondaryAxisEdgeLength.Title.Text = "Длина борта, сm";

                var textView = sender as TextView;
                textView.Text = string.Empty;
                textView = null;
            }


            //тултип не работает, что-то придумать
            void ChartEdgeLength_TooltipCreated(object sender, SfChart.TooltipCreatedEventArgs e)
            {
                var ser = e.P1.Series;
                var data = e.P1.Series.ItemsSource as List<PizzaList>;
                if (ser.SelectedDataPointIndex > -1)
                    e.P1.Label = "Quantity :" + data[ser.SelectedDataPointIndex].Quantity.ToString() + "\n Diameter :" + data[ser.SelectedDataPointIndex].diameter.ToString();
            }

            //-------------------------------------------------------------------------конец второго графика


            //------------------------------------------------------------------------третий график 

            SfChart chartPriceToArea = new SfChart(this);

            //Initializing primary axis
            CategoryAxis primaryAxisPriceToArea = new CategoryAxis();
            //primaryAxisPriceToArea.Title.Text = "Номер пиццы в списке"; //подпись 
            chartPriceToArea.PrimaryAxis = primaryAxisPriceToArea;
            chartPriceToArea.PrimaryAxis.AutoScrollingDelta = 4; //на графике 4 максимум
            chartPriceToArea.PrimaryAxis.AutoScrollingMode = ChartAutoScrollingMode.Start; //стартуем с начала при пролистыавании
            chartPriceToArea.PrimaryAxis.Interval = 1;

            chartPriceToArea.TooltipCreated += ChartPriceToArea_TooltipCreated;


            //Initializing secondary Axis
            NumericalAxis secondaryAxisPriceToArea = new NumericalAxis();
            secondaryAxisPriceToArea.Title.Text = "Цена за m²";
            chartPriceToArea.SecondaryAxis = secondaryAxisPriceToArea;

            //Initializing column series
            ColumnSeries seriesPriceToArea = new ColumnSeries();
            seriesPriceToArea.ItemsSource = compares;
            seriesPriceToArea.XBindingPath = "Name";
            seriesPriceToArea.YBindingPath = "PriceToArea";
            seriesPriceToArea.ColorModel.ColorPalette = ChartColorPalette.Metro;//цвет
            seriesPriceToArea.DataMarker.LabelStyle.LabelFormat = "#.#### $";//формат чисел
            seriesPriceToArea.DataMarkerLabelCreated += SeriesPriceToArea_DataMarkerLabelCreated;

            seriesPriceToArea.DataPointSelectionEnabled = true;//выбрать столбик

            //ивент выбора столбика, когда столбюик выбран считаем по методу расчеты в % и показываем их, когда столбик не выбран показываем стадартные рассчеты
            int idxPriceToArea = 0;

            chartPriceToArea.SelectionChanged += Chart_SelectionChangedchartPriceToArea;

            void Chart_SelectionChangedchartPriceToArea(object sender, SfChart.SelectionChangedEventArgs e)
            {

                if (e.P1.SelectedDataPointIndex > -1)
                {
                    int index = seriesPriceToArea.SelectedDataPointIndex;

                    while (idxPriceToArea < compares.Count())
                    {
                        if (compares[index].PriceToArea != 0)
                        {
                            compares[idxPriceToArea].InPercent = Math.Round((compares[idxPriceToArea].PriceToArea / compares[index].PriceToArea) * 100 - 100, 2);
                        }

                        idxPriceToArea++;
                    }
                    seriesPriceToArea.YBindingPath = "InPercent";
                    seriesPriceToArea.DataMarker.LabelStyle.LabelFormat = "#.##'%'";//формат чисел
                    secondaryAxisPriceToArea.Title.Text = "Сравнение в %";
                    seriesPriceToArea.SelectedDataPointIndex = -1;
                }
                else
                {
                    idxPriceToArea = 0;
                    seriesPriceToArea.YBindingPath = "PriceToArea";
                    seriesPriceToArea.DataMarker.LabelStyle.LabelFormat = "#.#### $";//формат чисел
                    secondaryAxisPriceToArea.Title.Text = "Цена за m²";
                }
            }

            seriesPriceToArea.DataMarker.ShowLabel = true;
            seriesPriceToArea.Label = "Цена в $ за m²";
            seriesPriceToArea.TooltipEnabled = true;
            //chartPriceToArea.Title.Text = "Нажмите на столбец для сравнения в %";


            ChartZoomPanBehavior zoomPriceToArea = new ChartZoomPanBehavior();// скрол в сторону
            zoomPriceToArea.DoubleTapEnabled = false;
            zoomPriceToArea.ZoomingEnabled = false;
            zoomPriceToArea.ScrollingEnabled = true;
            chartPriceToArea.Behaviors.Add(zoomPriceToArea);

            chartPriceToArea.Series.Add(seriesPriceToArea);
            chartPriceToArea.Legend.Visibility = Visibility.Visible;
            FindViewById<LinearLayout>(Resource.Id.LayoutChartPriceToArea).AddView(chartPriceToArea);

            void SeriesPriceToArea_DataMarkerLabelCreated(object sender, ChartSeries.DataMarkerLabelCreatedEventArgs e)
            {
                var data = e.DataMarkerLabel.Data as CompareList;
                if (data != null && (chartPriceToArea.Series[0] as ColumnSeries).YBindingPath == "InPercent" && data.InPercent == 0)
                {
                    TextView textPriceToArea = new TextView(this) { Text = "0 %" };
                    textPriceToArea.SetTextColor(Color.Black);
                    textPriceToArea.Click += TextPriceToArea_Click;
                    e.DataMarkerLabel.View = textPriceToArea;
                }
            }

            void TextPriceToArea_Click(object sender, EventArgs e)
            {
                idxPriceToArea = 0;
                seriesPriceToArea.YBindingPath = "PriceToArea";
                seriesPriceToArea.DataMarker.LabelStyle.LabelFormat = "#.#### $";
                seriesPriceToArea.SelectedDataPointIndex = -1;
                secondaryAxisPriceToArea.Title.Text = "Цена за m²";

                var textView = sender as TextView;
                textView.Text = string.Empty;
                textView = null;
            }


            //тултип не работает, что-то придумать
            void ChartPriceToArea_TooltipCreated(object sender, SfChart.TooltipCreatedEventArgs e)
            {
                var ser = e.P1.Series;
                var data = e.P1.Series.ItemsSource as List<PizzaList>;
                if (ser.SelectedDataPointIndex > -1)
                    e.P1.Label = "Quantity :" + data[ser.SelectedDataPointIndex].Quantity.ToString() + "\n Diameter :" + data[ser.SelectedDataPointIndex].diameter.ToString();
            }
            //-------------------------------------------------------------------------конец третьего графика

            //------------------------------------------------------------------------четвертый график 

            SfChart chartPriceToWeight = new SfChart(this);

            //Initializing primary axis
            CategoryAxis primaryAxisPriceToWeight = new CategoryAxis();
            //primaryAxisPriceToWeight.Title.Text = "Номер пиццы в списке"; //подпись 
            chartPriceToWeight.PrimaryAxis = primaryAxisPriceToWeight;
            chartPriceToWeight.PrimaryAxis.AutoScrollingDelta = 4; //на графике 4 максимум
            chartPriceToWeight.PrimaryAxis.AutoScrollingMode = ChartAutoScrollingMode.Start; //стартуем с начала при пролистыавании
            chartPriceToWeight.PrimaryAxis.Interval = 1;


            chartPriceToWeight.TooltipCreated += ChartPriceToWeight_TooltipCreated;


            //Initializing secondary Axis
            NumericalAxis secondaryAxisPriceToWeight = new NumericalAxis();
            secondaryAxisPriceToWeight.Title.Text = "Цена за кг";
            chartPriceToWeight.SecondaryAxis = secondaryAxisPriceToWeight;

            //Initializing column series
            ColumnSeries seriesPriceToWeight = new ColumnSeries();
            seriesPriceToWeight.ItemsSource = compares;
            seriesPriceToWeight.XBindingPath = "Name";
            seriesPriceToWeight.YBindingPath = "PriceToWeight";
            seriesPriceToWeight.ColorModel.ColorPalette = ChartColorPalette.Metro;//цвет
            seriesPriceToWeight.DataMarker.LabelStyle.LabelFormat = "#.#### $";//формат чисел
            seriesPriceToWeight.DataMarkerLabelCreated += SeriesPriceToWeight_DataMarkerLabelCreated;

            seriesPriceToWeight.DataPointSelectionEnabled = true;//выбрать столбик

            //ивент выбора столбика, когда столбюик выбран считаем по методу расчеты в % и показываем их, когда столбик не выбран показываем стадартные рассчеты
            int idxPriceToWeight = 0;

            chartPriceToWeight.SelectionChanged += Chart_SelectionChangedchartPriceToWeight;

            void Chart_SelectionChangedchartPriceToWeight(object sender, SfChart.SelectionChangedEventArgs e)
            {

                if (e.P1.SelectedDataPointIndex > -1)
                {
                    int index = seriesPriceToWeight.SelectedDataPointIndex;

                    while (idxPriceToWeight < compares.Count())
                    {
                        if (compares[index].PriceToWeight != 0)
                        {
                            compares[idxPriceToWeight].InPercent = Math.Round((compares[idxPriceToWeight].PriceToWeight / compares[index].PriceToWeight) * 100 - 100, 2);
                        }

                        idxPriceToWeight++;
                    }
                    seriesPriceToWeight.YBindingPath = "InPercent";
                    seriesPriceToWeight.DataMarker.LabelStyle.LabelFormat = "#.##'%'";//формат чисел
                    secondaryAxisPriceToWeight.Title.Text = "Сравнение в %";
                    seriesPriceToWeight.SelectedDataPointIndex = -1;
                }
                else
                {
                    idxPriceToWeight = 0;
                    seriesPriceToWeight.YBindingPath = "PriceToWeight";
                    seriesPriceToWeight.DataMarker.LabelStyle.LabelFormat = "#.#### $";//формат чисел
                    secondaryAxisPriceToWeight.Title.Text = "Цена за кг";
                }
            }

            seriesPriceToWeight.DataMarker.ShowLabel = true;
            seriesPriceToWeight.Label = "Цена в $ за 1 кг";
            seriesPriceToWeight.TooltipEnabled = true;
            //chartPriceToWeight.Title.Text = "Нажмите на столбец для сравнения в %";


            ChartZoomPanBehavior zoomPriceToWeight = new ChartZoomPanBehavior();// скрол в сторону
            zoomPriceToWeight.DoubleTapEnabled = false;
            zoomPriceToWeight.ZoomingEnabled = false;
            zoomPriceToWeight.ScrollingEnabled = true;
            chartPriceToWeight.Behaviors.Add(zoomPriceToWeight);

            chartPriceToWeight.Series.Add(seriesPriceToWeight);
            chartPriceToWeight.Legend.Visibility = Visibility.Visible;
            FindViewById<LinearLayout>(Resource.Id.LayoutChartPriceToWeight).AddView(chartPriceToWeight);

            void SeriesPriceToWeight_DataMarkerLabelCreated(object sender, ChartSeries.DataMarkerLabelCreatedEventArgs e)
            {
                var data = e.DataMarkerLabel.Data as CompareList;
                if (data != null && (chartPriceToWeight.Series[0] as ColumnSeries).YBindingPath == "InPercent" && data.InPercent == 0)
                {
                    TextView textPriceToWeight = new TextView(this) { Text = "0 %" };
                    textPriceToWeight.SetTextColor(Color.Black);
                    textPriceToWeight.Click += TextPriceToWeight_Click;
                    e.DataMarkerLabel.View = textPriceToWeight;
                }
            }

            void TextPriceToWeight_Click(object sender, EventArgs e)
            {
                idxPriceToWeight = 0;
                seriesPriceToWeight.YBindingPath = "PriceToWeight";
                seriesPriceToWeight.DataMarker.LabelStyle.LabelFormat = "#.#### $";
                seriesPriceToWeight.SelectedDataPointIndex = -1;
                secondaryAxisPriceToWeight.Title.Text = "Цена за кг";

                var textView = sender as TextView;
                textView.Text = string.Empty;
                textView = null;
            }


            //тултип не работает, что-то придумать
            void ChartPriceToWeight_TooltipCreated(object sender, SfChart.TooltipCreatedEventArgs e)
            {
                var ser = e.P1.Series;
                var data = e.P1.Series.ItemsSource as List<PizzaList>;
                if (ser.SelectedDataPointIndex > -1)
                    e.P1.Label = "Quantity :" + data[ser.SelectedDataPointIndex].Quantity.ToString() + "\n Diameter :" + data[ser.SelectedDataPointIndex].diameter.ToString();
            }
            //-------------------------------------------------------------------------конец четвертого графика
        }

    }
}