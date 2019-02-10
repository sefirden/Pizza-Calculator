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

namespace Pizza_Calculator
{
    [Activity(Label = "CompareActivity")]
    public class CompareActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Compare);

            string listAsString = Intent.GetStringExtra("saved_counter"); //эти две строки передают список пицц в это активити
            List<PizzaList> pizza = JsonConvert.DeserializeObject<List<PizzaList>>(listAsString);

            //добавляем расчеты в клас
            List<CompareList> compares = new List<CompareList>();

            int pizzanumber = 0;
            while (pizzanumber < pizza.Count())
            {
                compares.Add(new CompareList("pizza1", pizza[pizzanumber].GetArea(), pizza[pizzanumber].GetEdgeLength(), pizza[pizzanumber].PriceToArea(), pizza[pizzanumber].PriceToWeight()));
                pizzanumber++;
            }


            //первый график 
            SfChart chart = FindViewById<SfChart>(Resource.Id.sfChart1);
            chart = new SfChart(this);

            //Initializing primary axis
            CategoryAxis primaryAxis = new CategoryAxis();
            primaryAxis.Title.Text = "Pizza #";
            chart.PrimaryAxis = primaryAxis;

            //Initializing secondary Axis
            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Title.Text = "Area (in cm2)";
            chart.SecondaryAxis = secondaryAxis;

            //Initializing column series
            ColumnSeries series = new ColumnSeries();
            series.ItemsSource = compares;
            series.XBindingPath = "Name";
            series.YBindingPath = "Area";

            series.DataMarker.ShowLabel = true;
            series.Label = "Area";
            series.TooltipEnabled = true;

            chart.Series.Add(series);
            chart.Legend.Visibility = Visibility.Visible;
            SetContentView(chart);
            //конец первого графика
        
        }
    }
}