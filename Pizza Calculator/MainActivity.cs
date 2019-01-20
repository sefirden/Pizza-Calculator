using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace Pizza_Calculator
{
    [Activity(Label = "Pizza_Calculator", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            CheckBox question = FindViewById<CheckBox>(Resource.Id.checkBox1);
            ListView listView = FindViewById<ListView>(Resource.Id.listView1);

            //кнопки 
            Button addToList = FindViewById<Button>(Resource.Id.button1);
            Button compare = FindViewById<Button>(Resource.Id.button2);

            //поля для ввода данных
            EditText getQuantity = FindViewById<EditText>(Resource.Id.editText1);
            EditText getDiameter = FindViewById<EditText>(Resource.Id.editText2);
            EditText getPrice = FindViewById<EditText>(Resource.Id.editText3);
            EditText getWeight = FindViewById<EditText>(Resource.Id.editText4);

            //название полей и они же тестовые заплатки для проверки
            TextView test1 = FindViewById<TextView>(Resource.Id.textView1);
            TextView test2 = FindViewById<TextView>(Resource.Id.textView2);
            TextView test3 = FindViewById<TextView>(Resource.Id.textView3);
            TextView test4 = FindViewById<TextView>(Resource.Id.textView4);

            int quantity = 0;
            double diameter = 0;
            double price = 0;
            double weight = 0;

            List<Pizza> pizza = new List<Pizza>();

            ArrayAdapter<Pizza> adapter = new ArrayAdapter<Pizza>(this, Resource.Layout.list_item, pizza);
            listView.Adapter = adapter;

            addToList.Click += (o, e) =>
            {
                //начало сбора данных с полей, с  проверкой. 
                
                //сделать смену фокуса после сброса данных
                
                //quantity
                if (getQuantity.Text == "" || getQuantity.Text == null)
                { quantity = 0; }
                else { quantity = Convert.ToInt32(getQuantity.Text); }
                //diameter
                if (getDiameter.Text == "" || getDiameter.Text == null)
                { diameter = 0; }
                else { double.TryParse(getDiameter.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out diameter); }
                //price
                if (getPrice.Text == "" || getPrice.Text == null)
                { price = 0; }
                else { double.TryParse(getPrice.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out price); }
                //weight
                if (getWeight.Text == "" || getWeight.Text == null)
                { weight = 0; }
                else { double.TryParse(getWeight.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out weight); }
                //конец сбора данных

                //сброс данных в полях
                getQuantity.Text = null;
                getDiameter.Text = null;
                getPrice.Text = null;
                getWeight.Text = null;

                //фокус на 1 строку
                getQuantity.RequestFocus();

                //начало вывода данных для проверки, потом удалить
                test1.Text = Convert.ToString(quantity);
                test2.Text = Convert.ToString(diameter);
                test3.Text = Convert.ToString(price);
                test4.Text = Convert.ToString(weight);
                //конец вывода данных для проверки

                //из полученных данных создаем обьект списка
                pizza.Add(new Pizza(quantity, diameter, price, weight));

                //гдето тут надо написать добавление элемента в список ListView
            };

            compare.Click += (o, e) =>
            {
                test1.Text = Convert.ToString(pizza.Count);
            };

        }
    }
}