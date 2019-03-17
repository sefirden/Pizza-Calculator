using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using Android.Content;
using Newtonsoft.Json;
using Com.Syncfusion.Charts;
using Android.Views;
using Android.Content.PM;
using System.Threading;


namespace Pizza_Calculator
//некоторую часть кода я сам не до конца понимаю, но пока все работает как планировалось
{

    [Activity(Theme = "@style/MyTheme.Launcher", ScreenOrientation = ScreenOrientation.Portrait, Label = "@string/app_name", MainLauncher = true)]

    public class MainActivity : Activity

    {

        private RecyclerView recyclerview;
        RecyclerView.LayoutManager recyclerview_layoutmanger;
        private RecyclerView.Adapter recyclerview_adapter;
        private PizzaListAdapter<PizzaList> PizzaListitems;
        List<PizzaList> pizza = new List<PizzaList>();
        FloatingActionButton fabAdd;
        FloatingActionButton fabCompare;
        TextView nodata;
        int listNumber = 0;  //это номер пиццы в списке для отображения
        int quantity;
        double diameter = 0;
        double price = 0;
        double weight = 0;

        long lastPress;

        public override void OnBackPressed()
        {

            long currentTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            if (currentTime - lastPress > 5000)
            {
                Toast.MakeText(this, "Нажмите еще раз для выхода", ToastLength.Long).Show();
                lastPress = currentTime;
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnCreate(Bundle bundle)
        {

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njc1MDVAMzEzNjJlMzQyZTMwa2VZM2hTaE1FNXlyOU0yeUVtVXZmNm5HMnhRQjdWTHNWSk51ZGMxL3p5Zz0="); //лицензия на графики

            SetTheme(Resource.Style.MyTheme);

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            recyclerview = FindViewById<RecyclerView>(Resource.Id.recyclerview);//область прокрутки
            //текст при пустом списке
            recyclerview.SetItemAnimator(new DefaultItemAnimator());

            //кнопки 
            fabAdd = FindViewById<FloatingActionButton>(Resource.Id.add);
            fabCompare = FindViewById<FloatingActionButton>(Resource.Id.compare);
            nodata = FindViewById<TextView>(Resource.Id.empty_view); //текст без данных
            
            //список картинок ниже

            string[] photolist = { "pizza1", "pizza2", "pizza3" }; //вписываем названия картинок

            var rnd = new Random();
            photolist = photolist.OrderBy(s => rnd.Next()).ToArray();//перемешивем список фоток 

            PizzaListitems = new PizzaListAdapter<PizzaList>(); //это перенес из под кнопки


            int i = 0; // индекс для картинок
            int pic; //для передачи картинки в список
            int count;

            fabAdd.Click += delegate {
                LayoutInflater layoutInflater = LayoutInflater.From(this);
                View view = layoutInflater.Inflate(Resource.Layout.user_input_dialog_box, null);
                Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertbuilder.SetView(view);
                var getQuantity = view.FindViewById<EditText>(Resource.Id.edit_Quantity);
                var getDiameter = view.FindViewById<EditText>(Resource.Id.edit_Diameter);
                var getPrice = view.FindViewById<EditText>(Resource.Id.edit_Price);
                var getWeight = view.FindViewById<EditText>(Resource.Id.edit_Weight); 


            alertbuilder.SetCancelable(false)
                .SetPositiveButton("Submit", delegate
                {
                                                         
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

                    string pictureName = photolist[i]; //присваиваем значение из перемешенного списка фоток
                    pic = Resources.GetIdentifier(pictureName, "drawable", PackageName); //присваиваем параметру pic картинку, используя название картинки из списка


                    //из полученных данных создаем обьект списка
                    pizza.Add(new PizzaList(quantity, diameter, price, weight, pic));

                    PizzaListitems.Add(pizza[listNumber]);
                    recyclerview_adapter.NotifyDataSetChanged();
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

                    // эти две строки переназначают адаптер, это обновляет список, но не совсем верно. На больших списках не использовать, пофиксить позже

                    //включаем кнопку сравнить и убираем текст заменяя списком

                    count = pizza.Count();

                    if (count <= 1)
                    {
                        nodata.Visibility = ViewStates.Gone;
                        fabCompare.Visibility = ViewStates.Visible;
                        recyclerview.Visibility = ViewStates.Visible;
                    }



                })
                .SetNegativeButton("Cancel", delegate
                {
                    alertbuilder.Dispose();
                });
                Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
                dialog.Show();
            };

            //конец диалогового окна и добавления пицы в список


            //fabCompare.Click += Button_Click; //запускаем активити со сравнением   
            
                    //кусок тут
        void ShowProgressBar(bool show)
        {
            RunOnUiThread(() => {
                ProgressBar probar = FindViewById<ProgressBar>(Resource.Id.progressbar);
                probar.Visibility = show ? ViewStates.Visible : ViewStates.Invisible;
            });
        }
            fabCompare.Click += delegate {
                ShowProgressBar(true);
                ThreadStart th = new ThreadStart(Button_Click);
                Thread myThread = new Thread(th);
                myThread.Start();
            };

            void Button_Click()
            {
                var intent = new Intent(this, typeof(CompareActivity));
                string listAsString = JsonConvert.SerializeObject(pizza);
                intent.PutExtra("saved_counter", listAsString);
                this.StartActivity(intent);
                ShowProgressBar(false);
            }


            recyclerview_layoutmanger = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerview.SetLayoutManager(recyclerview_layoutmanger);
            recyclerview_adapter = new RecyclerAdapter(PizzaListitems, this);
            recyclerview.SetAdapter(recyclerview_adapter);

        }

        public void Delete(int position)
        {
            pizza.RemoveAt(position);
            listNumber--;

            int count = pizza.Count();

            if (count == 0)
            {
                nodata.Visibility = ViewStates.Visible;
                fabCompare.Visibility = ViewStates.Gone;
                recyclerview.Visibility = ViewStates.Gone;
            }
        }

        public void Edit(int position, int num)
        {
            LayoutInflater layoutInflater = LayoutInflater.From(this);
            View view = layoutInflater.Inflate(Resource.Layout.user_input_dialog_box, null);
            Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertbuilder.SetView(view);
            var getQuantity = view.FindViewById<EditText>(Resource.Id.edit_Quantity);
            var getDiameter = view.FindViewById<EditText>(Resource.Id.edit_Diameter);
            var getPrice = view.FindViewById<EditText>(Resource.Id.edit_Price);
            var getWeight = view.FindViewById<EditText>(Resource.Id.edit_Weight);

            getQuantity.Text = Convert.ToString(pizza[position].quantity);
            getDiameter.Text = Convert.ToString(pizza[position].diameter);
            getPrice.Text = Convert.ToString(pizza[position].price);
            getWeight.Text = Convert.ToString(pizza[position].weight);
                                 
            alertbuilder.SetCancelable(false)
                .SetPositiveButton("Submit", delegate
                {

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

                    pizza[position].Quantity = quantity;
                    pizza[position].diameter = diameter;
                    pizza[position].price = price;
                    pizza[position].weight = weight;

                    Toast.MakeText(Application.Context, "Pizza #" + num.ToString() + " edited!", ToastLength.Short).Show();
                    recyclerview_adapter.NotifyItemChanged(position);

                    // эти две строки переназначают адаптер, это обновляет список, но не совсем верно. На больших списках не использовать, пофиксить позже



                })
                .SetNegativeButton("Cancel", delegate
                {
                    alertbuilder.Dispose();
                });
            Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
            dialog.Show();
        }

        //конец диалогового окна и добавления пицы в список
    }
}