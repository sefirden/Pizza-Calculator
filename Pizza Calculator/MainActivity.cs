using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;


namespace Pizza_Calculator
//некоторую часть кода я сас не до конца понимаю, но пока все работает как планировалось
{

    [Activity(Label = "@string/app_name", MainLauncher = true)]

    public class MainActivity : Activity

    {

        private RecyclerView recyclerview;
        RecyclerView.LayoutManager recyclerview_layoutmanger;
        private RecyclerView.Adapter recyclerview_adapter;
        private PizzaListAdapter<PizzaList> PizzaListitems;
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            recyclerview = FindViewById<RecyclerView>(Resource.Id.recyclerview);//область прокрутки

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

            //список картинок ниже

            string[] photolist = { "pizza1", "pizza2", "pizza3" }; //вписываем названия картинок

            var rnd = new Random();
            photolist = photolist.OrderBy(s => rnd.Next()).ToArray();//перемешивем список фоток 

            List<PizzaList> pizza = new List<PizzaList>();
            PizzaListitems = new PizzaListAdapter<PizzaList>(); //это перенес из под кнопки

            // тут дописал----------
            var listNumber = 0;  //это номер пиццы в списке для отображения
            int quantity = 0;
            double diameter = 0;
            double price = 0;
            double weight = 0;
            int i = 0; // индекс для картинок
            int pic; //для передачи картинки в список

            ///// клик на кнопку "Add to list"
            addToList.Click += (o, e) =>
            {


                //начало сбора данных с полей, если поле пустое или null, то присваиваем 0 

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

                string pictureName = photolist[i]; //присваиваем значение из перемашанного списка фоток
                pic = Resources.GetIdentifier(pictureName, "drawable", PackageName); //присваиваем параметру pic картинку, используя название картинки из списка


                //из полученных данных создаем обьект списка
                pizza.Add(new PizzaList(quantity, diameter, price, weight, pic));

                PizzaListitems.Add(pizza[listNumber]);
                //добаляем елемент в список на позицию 0, и на удивление это сработало :)

                listNumber++; //каждый раз после добавления увеличиваем позицию на 1   

                if (i < photolist.Length - 1) //если количество обьектов в списке будет больше чем список фоток, то еще раз перемешиваем список фоток
                {
                    i++; //следующая фотка из списка
                }
                else
                {
                    rnd = new Random();
                    photolist = photolist.OrderBy(s => rnd.Next()).ToArray();//перемешивем список фоток
                    i = 0; //индекс возвращаем к 0
                }

                recyclerview_adapter = new RecyclerAdapter(PizzaListitems);
                recyclerview.SetAdapter(recyclerview_adapter);
                // эти две строки переназначают адаптер, это обновляет список, но не совсем верно. На больших списках не использовать, пофиксить позже
            };


            compare.Click += (o, e) =>
            {
                test1.Text = Convert.ToString(pizza.Count);
                test2.Text = Convert.ToString(PizzaListitems.Count);
            };
            ////конец


            recyclerview_layoutmanger = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerview.SetLayoutManager(recyclerview_layoutmanger);
            recyclerview_adapter = new RecyclerAdapter(PizzaListitems);
            recyclerview.SetAdapter(recyclerview_adapter);

        }
    }
}