using System;
using Android.Support.V7.App;
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
using Android.Content.Res;

namespace Pizza_Calculator
{
    [Activity(Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait, Label = "CompareActivity")]
    public class CompareActivity : AppCompatActivity
    {
        //добавляем расчеты в клас
        Context context = Application.Context;

        string piz_num;
        string pizza_sel_no_edge;
        string percent_y;
        string pizza_sel;
        string pizza_x_q;
        string pizza_x_d;
        string pizza_x_p;
        string pizza_x_w;
        string area_y;
        string area_y_label;
        string area_title;
        string length_y;
        string length_y_label;
        string length_title;
        string price_area_y;
        string price_area_y_label;
        string price_area_title;
        string price_weight_y;
        string price_weight_y_label;
        string price_weight_title;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            //лицензия на графики из второго активити
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njc1MDVAMzEzNjJlMzQyZTMwa2VZM2hTaE1FNXlyOU0yeUVtVXZmNm5HMnhRQjdWTHNWSk51ZGMxL3p5Zz0=");
            SetContentView(Resource.Layout.Compare);

            //все фразы ниже
           piz_num = Resources.GetText(Resource.String.piz_num);
           percent_y = Resources.GetText(Resource.String.percent_y);
           pizza_sel = Resources.GetText(Resource.String.pizza_sel);
           pizza_x_q = Resources.GetText(Resource.String.pizza_x_q);
           pizza_x_d = Resources.GetText(Resource.String.pizza_x_d);
           pizza_x_p = Resources.GetText(Resource.String.pizza_x_p);
           pizza_x_w = Resources.GetText(Resource.String.pizza_x_w);
           area_y = Resources.GetText(Resource.String.area_y);
           area_y_label = Resources.GetText(Resource.String.area_y_label);
           area_title = Resources.GetText(Resource.String.area_title);
           length_y = Resources.GetText(Resource.String.length_y);
           length_y_label = Resources.GetText(Resource.String.length_y_label);
           length_title = Resources.GetText(Resource.String.length_title);
           price_area_y = Resources.GetText(Resource.String.price_area_y);
           price_area_y_label = Resources.GetText(Resource.String.price_area_y_label);
           price_area_title = Resources.GetText(Resource.String.price_area_title);
           price_weight_y = Resources.GetText(Resource.String.price_weight_y);
           price_weight_y_label = Resources.GetText(Resource.String.price_weight_y_label);
           price_weight_title = Resources.GetText(Resource.String.price_weight_title);
           pizza_sel_no_edge = Resources.GetText(Resource.String.pizza_sel_no_edge);

            string listAsString = Intent.GetStringExtra("saved_counter"); //эти две строки передают список пицц в это активити
            List<PizzaList> pizza = JsonConvert.DeserializeObject<List<PizzaList>>(listAsString);

            List<CompareList> compares = new List<CompareList>();
            int pizzanumber = 0;
            while (pizzanumber < pizza.Count())
            {
                compares.Add(new CompareList(piz_num + Convert.ToString(pizzanumber + 1), pizza[pizzanumber].GetArea(), pizza[pizzanumber].GetEdgeLength(), pizza[pizzanumber].PriceToArea(), pizza[pizzanumber].PriceToWeight(), 10));
                pizzanumber++;
            }


            //------------------------------------------------------------------------первый график 
            SfChart chartArea = new SfChart(this);

            //Initializing primary axis
            CategoryAxis primaryAxisArea = new CategoryAxis();
            chartArea.PrimaryAxis = primaryAxisArea;
            chartArea.PrimaryAxis.AutoScrollingDelta = 4; //на графике 4 максимум
            chartArea.PrimaryAxis.AutoScrollingMode = ChartAutoScrollingMode.Start; //стартуем с начала при пролистыавании
            chartArea.PrimaryAxis.Interval = 1;

            primaryAxisArea.Title.Text = " ";

            //Initializing secondary Axis
            NumericalAxis secondaryAxisArea = new NumericalAxis();
            secondaryAxisArea.Title.Text = area_y;
            chartArea.SecondaryAxis = secondaryAxisArea;

            //Initializing column series
            ColumnSeries seriesArea = new ColumnSeries();
            seriesArea.ItemsSource = compares;
            seriesArea.XBindingPath = "Name";
            seriesArea.YBindingPath = "Area";
            seriesArea.ColorModel.ColorPalette = ChartColorPalette.Metro;//цвет
            seriesArea.DataMarker.LabelStyle.LabelFormat = area_y_label;//формат чисел
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
                    secondaryAxisArea.Title.Text = percent_y;

                    var number = index + 1;

                    if (pizza[index].diameterNoEdge > 0)
                    {
                        primaryAxisArea.Title.Text = pizza_sel_no_edge + number.ToString() + pizza_x_q + pizza[index].Quantity.ToString() + pizza_x_d + pizza[index].diameterNoEdge.ToString();
                    }
                    else
                    {
                        primaryAxisArea.Title.Text = pizza_sel + number.ToString() + pizza_x_q + pizza[index].Quantity.ToString() + pizza_x_d + pizza[index].diameter.ToString();  
                    }

                    seriesArea.SelectedDataPointIndex = -1;
                }
                else
                {
                    idxArea = 0;
                    seriesArea.YBindingPath = "Area";
                    seriesArea.DataMarker.LabelStyle.LabelFormat = area_y_label;//формат чисел
                    secondaryAxisArea.Title.Text = area_y;
                    primaryAxisArea.Title.Text = " ";//подсказка!!

                }
            }

            seriesArea.DataMarker.ShowLabel = true;
            seriesArea.Label = area_title;


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
                    TextView textArea = new TextView(this) { Text = "0 %" };
                    textArea.SetTextColor(Color.Black);
                    textArea.Click += TextArea_Click;
                    e.DataMarkerLabel.View = textArea;
                }
            }

            void TextArea_Click(object sender, EventArgs e)
            {
                idxArea = 0;
                seriesArea.YBindingPath = "Area";
                seriesArea.DataMarker.LabelStyle.LabelFormat = area_y_label;
                seriesArea.SelectedDataPointIndex = -1;
                secondaryAxisArea.Title.Text = area_y;
                primaryAxisArea.Title.Text = " ";//подсказка!!

                var textView = sender as TextView;
                textView.Text = string.Empty;
                textView = null;
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
            primaryAxisEdgeLength.Title.Text = " ";


            //Initializing secondary Axis
            NumericalAxis secondaryAxisEdgeLength = new NumericalAxis();
            secondaryAxisEdgeLength.Title.Text = length_y;
            chartEdgeLength.SecondaryAxis = secondaryAxisEdgeLength;

            //Initializing column series
            ColumnSeries seriesEdgeLength = new ColumnSeries();
            seriesEdgeLength.ItemsSource = compares;
            seriesEdgeLength.XBindingPath = "Name";
            seriesEdgeLength.YBindingPath = "EdgeLength";
            seriesEdgeLength.ColorModel.ColorPalette = ChartColorPalette.Metro;//цвет
            seriesEdgeLength.DataMarker.LabelStyle.LabelFormat = length_y_label;//формат чисел
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
                    secondaryAxisEdgeLength.Title.Text = percent_y;

                    var number = index + 1;
                    primaryAxisEdgeLength.Title.Text = pizza_sel + number.ToString() + pizza_x_q + pizza[index].Quantity.ToString() + pizza_x_d + pizza[index].diameter.ToString();//подсказка!!

                    seriesEdgeLength.SelectedDataPointIndex = -1;

                }
                else
                {
                    idxEdgeLength = 0;
                    seriesEdgeLength.YBindingPath = "EdgeLength";
                    seriesEdgeLength.DataMarker.LabelStyle.LabelFormat = length_y_label;//формат чисел
                    secondaryAxisEdgeLength.Title.Text = length_y;
                    primaryAxisEdgeLength.Title.Text = " ";//подсказка!!
                }
            }

            seriesEdgeLength.DataMarker.ShowLabel = true;
            seriesEdgeLength.Label = length_title;
            seriesEdgeLength.TooltipEnabled = true;



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
                seriesEdgeLength.DataMarker.LabelStyle.LabelFormat = length_y_label;
                seriesEdgeLength.SelectedDataPointIndex = -1;
                secondaryAxisEdgeLength.Title.Text = length_y;
                primaryAxisEdgeLength.Title.Text = " ";//подсказка!!

                var textView = sender as TextView;
                textView.Text = string.Empty;
                textView = null;
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
            primaryAxisPriceToArea.Title.Text = " ";


            //Initializing secondary Axis
            NumericalAxis secondaryAxisPriceToArea = new NumericalAxis();
            secondaryAxisPriceToArea.Title.Text = price_area_y;
            chartPriceToArea.SecondaryAxis = secondaryAxisPriceToArea;

            //Initializing column series
            ColumnSeries seriesPriceToArea = new ColumnSeries();
            seriesPriceToArea.ItemsSource = compares;
            seriesPriceToArea.XBindingPath = "Name";
            seriesPriceToArea.YBindingPath = "PriceToArea";
            seriesPriceToArea.ColorModel.ColorPalette = ChartColorPalette.Metro;//цвет
            seriesPriceToArea.DataMarker.LabelStyle.LabelFormat = price_area_y_label;//формат чисел
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
                    secondaryAxisPriceToArea.Title.Text = percent_y;

                    var number = index + 1;

                    string text_info;
                    if (pizza[index].diameterNoEdge > 0)
                    {
                        text_info = pizza_sel_no_edge + number.ToString() + pizza_x_d + pizza[index].diameterNoEdge.ToString() + pizza_x_p + pizza[index].price.ToString();
                    }
                    else
                    {
                        text_info = pizza_sel + number.ToString() + pizza_x_d + pizza[index].diameter.ToString() + pizza_x_p + pizza[index].price.ToString();  
                    }
                    primaryAxisPriceToArea.Title.Text = text_info;//подсказка!!

                    seriesPriceToArea.SelectedDataPointIndex = -1;
                }
                else
                {
                    idxPriceToArea = 0;
                    seriesPriceToArea.YBindingPath = "PriceToArea";
                    seriesPriceToArea.DataMarker.LabelStyle.LabelFormat = price_area_y_label;//формат чисел
                    secondaryAxisPriceToArea.Title.Text = price_area_y;
                    primaryAxisPriceToArea.Title.Text = " ";
                }
            }

            seriesPriceToArea.DataMarker.ShowLabel = true;
            seriesPriceToArea.Label = price_area_title;


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
                seriesPriceToArea.DataMarker.LabelStyle.LabelFormat = price_area_y_label;
                seriesPriceToArea.SelectedDataPointIndex = -1;
                secondaryAxisPriceToArea.Title.Text = price_area_y;
                primaryAxisPriceToArea.Title.Text = " ";

                var textView = sender as TextView;
                textView.Text = string.Empty;
                textView = null;
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
            primaryAxisPriceToWeight.Title.Text = " ";


            //Initializing secondary Axis
            NumericalAxis secondaryAxisPriceToWeight = new NumericalAxis();
            secondaryAxisPriceToWeight.Title.Text = price_weight_y;
            chartPriceToWeight.SecondaryAxis = secondaryAxisPriceToWeight;

            //Initializing column series
            ColumnSeries seriesPriceToWeight = new ColumnSeries();
            seriesPriceToWeight.ItemsSource = compares;
            seriesPriceToWeight.XBindingPath = "Name";
            seriesPriceToWeight.YBindingPath = "PriceToWeight";
            seriesPriceToWeight.ColorModel.ColorPalette = ChartColorPalette.Metro;//цвет
            seriesPriceToWeight.DataMarker.LabelStyle.LabelFormat = price_weight_y_label;//формат чисел
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
                    secondaryAxisPriceToWeight.Title.Text = percent_y;

                    var number = index + 1;

                    //string text_info;
                    if (pizza[index].weightNoEdge > 0)
                    {
                        primaryAxisPriceToWeight.Title.Text = pizza_sel_no_edge + number.ToString() + pizza_x_p + pizza[index].price.ToString() + pizza_x_w + pizza[index].weightNoEdge.ToString();
                        //primaryAxisPriceToWeight.Title.TextColor = Color.Red;
                    }
                    else
                    {
                        primaryAxisPriceToWeight.Title.Text = pizza_sel + number.ToString() + pizza_x_p + pizza[index].price.ToString() + pizza_x_w + pizza[index].weight.ToString();
                        //primaryAxisPriceToWeight.Title.TextColor = Color.Black;
                    }
                    //primaryAxisPriceToWeight.Title.Text = text_info;//подсказка!!

                    seriesPriceToWeight.SelectedDataPointIndex = -1;
                }
                else
                {
                    idxPriceToWeight = 0;
                    seriesPriceToWeight.YBindingPath = "PriceToWeight";
                    seriesPriceToWeight.DataMarker.LabelStyle.LabelFormat = price_weight_y_label;//формат чисел
                    secondaryAxisPriceToWeight.Title.Text = price_weight_y;
                    primaryAxisPriceToWeight.Title.Text = " ";
                }
            }

            seriesPriceToWeight.DataMarker.ShowLabel = true;
            seriesPriceToWeight.Label = price_weight_title;

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
                seriesPriceToWeight.DataMarker.LabelStyle.LabelFormat = price_weight_y_label;
                seriesPriceToWeight.SelectedDataPointIndex = -1;
                secondaryAxisPriceToWeight.Title.Text = price_weight_y;
                primaryAxisPriceToWeight.Title.Text = " ";

                var textView = sender as TextView;
                textView.Text = string.Empty;
                textView = null;
            }
            //-------------------------------------------------------------------------конец четвертого графика
        }

    }
}
