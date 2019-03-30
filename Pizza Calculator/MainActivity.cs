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
using Android.Content.Res;



namespace Pizza_Calculator
//некоторую часть кода я сам не до конца понимаю, но пока все работает как планировалось
{
    //устанавливаем тему Laucher для splach screen, фиксируем ориентацию экрана в портретном режиме, присваиваем название активити и еще какая-то херня
    [Activity(Theme = "@style/MyTheme.Launcher", ScreenOrientation = ScreenOrientation.Portrait, Label = "@string/app_name", MainLauncher = true)]

    public class MainActivity : AppCompatActivity //если пишем просто Activity то кастомный шрифт не работает

    {
        ///обьявляем переменные до старта активити

        //ниже все что связанно с recyclerview и списком пицц        
        private RecyclerView recyclerview;
        RecyclerView.LayoutManager recyclerview_layoutmanger;
        private RecyclerView.Adapter recyclerview_adapter;
        private PizzaListAdapter<PizzaList> PizzaListitems;
        List<PizzaList> pizza = new List<PizzaList>();
        //плавающие кнопки добавить и сравнить
        FloatingActionButton fabAdd;
        FloatingActionButton fabCompare;
        //контекст, незнаю зачем его тут писал, но возможно он где-то нужен
        Context context = Application.Context;
        //прогресс бар при загрузке активити с графиками
        ProgressBar probar;
        //текстовая подсказка, когда не добавлено ни одной пиццы
        TextView nodata;
        //номер пиццы в списке для отображения, где-то скорее всего есть другая переменная для этих же целей, поискать
        int listNumber = 0;
        //переменная для получения текущего количества пицц
        int count;
        //заглушки для создания пиццы, если человек не введет ничего в диалоговое окно, например он не знает вес  
        int quantity; //если ничего не введено то ставит 1, см класс Pizza
        double diameter = 0;
        double price = 0;
        double weight = 0;

        //все фразы, которые используются в коде объявляем ниже, далее в коде тянем их из @string/
        string exit_tap;
        string dialog_add;
        string dialog_ok;
        string dialog_canc;
        string piz_num;
        string toast_edit;
        string toast_del;
        ///переменные закончились

        //реализация кнопки назад в главном активити, чтобы при нажатии выскакивало предупреждение и только при повторном нажатии закрывать приложение. Писал не я, взято из интернетов, примерно понимаю как оно работает
        long lastPress;
        public override void OnBackPressed()
        {
            long currentTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond; //текущее время
            if (currentTime - lastPress > 5000)     //если текущее время больше чем время последнего нажатия больше чем на 5 сек
            {
                Toast.MakeText(this, exit_tap, ToastLength.Long).Show(); //показываем предупреждение
                lastPress = currentTime;
            }
            else //ну а если меньше 5 сек, то закрываем приложение
            {
                base.OnBackPressed();
            }
        } //конец кнопки назад

        //метод OnCreate
        protected override void OnCreate(Bundle bundle)
        {
            //лицензия на графики из второго активити
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njc1MDVAMzEzNjJlMzQyZTMwa2VZM2hTaE1FNXlyOU0yeUVtVXZmNm5HMnhRQjdWTHNWSk51ZGMxL3p5Zz0=");

            //меняем тему с splash screen на стандартную перед показом активити
            SetTheme(Resource.Style.MyTheme);

            base.OnCreate(bundle);
            // устанавливаем layout с именем Main
            SetContentView(Resource.Layout.Main);

            //присваиваем string'овым переменным фразы из @string/
            exit_tap = Resources.GetText(Resource.String.exit_tap); //если перестало работать, вернуть context.Resources.GetString
            dialog_add = Resources.GetText(Resource.String.dialog_add);
            dialog_ok = Resources.GetText(Resource.String.dialog_ok);
            dialog_canc = Resources.GetText(Resource.String.dialog_canc);
            piz_num = Resources.GetText(Resource.String.piz_num);
            toast_edit = Resources.GetText(Resource.String.toast_edit);
            toast_del = Resources.GetText(Resource.String.toast_del);

            //присваиваем переменным элементы из layout с именем Main
            //собственно сам reyclerview
            recyclerview = FindViewById<RecyclerView>(Resource.Id.recyclerview);
            //добавляем recyckerview стандартную анимацию
            recyclerview.SetItemAnimator(new DefaultItemAnimator());
           
            //прогресбар при смене на активити с графиками
            probar = FindViewById<ProgressBar>(Resource.Id.progressbar);
            //кнопки добавить и сравнить
            fabAdd = FindViewById<FloatingActionButton>(Resource.Id.add);
            fabCompare = FindViewById<FloatingActionButton>(Resource.Id.compare);
            //текст с подсказкой когда не добавленна ни одна пицца
            nodata = FindViewById<TextView>(Resource.Id.empty_view);

            //это скорее всего список пицц, который мы передаем в recyclerview adapter
            PizzaListitems = new PizzaListAdapter<PizzaList>();

            //супер хитрый способ рандомно смешивать фотки, для добавления их как элемента объекта Pizza, лучше ничего не исправлять
            //вписываем в масив названия всех картинок с пиццами
            string[] photolist = { "pizza1", "pizza2", "pizza3", "pizza4", "pizza5", "pizza6", "pizza7", "pizza8", "pizza9" };

            //перемешивем список фоток
            var rnd = new Random();
            photolist = photolist.OrderBy(s => rnd.Next()).ToArray();

            int i = 0; //индекс для подставления картинок из масива photolist
            int pic; //для передачи картинки в список, оно передает константу из цифр (непонятно почему именно так)

            ///ниже охереть какой большой кусок кода для нажатия по кнопке добавить пиццу в список fabAdd 
            fabAdd.Click += delegate {
                //создаем для диалогового окна новый view и присваиваем ему layout с именем user_input_dialog_box
                LayoutInflater layoutInflater = LayoutInflater.From(this);
                View view = layoutInflater.Inflate(Resource.Layout.user_input_dialog_box, null);
                Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertbuilder.SetView(view);
                //создаем переменные и присваиваем им элементы из layout с именем user_input_dialog_box
                var getQuantity = view.FindViewById<EditText>(Resource.Id.edit_Quantity);
                var getDiameter = view.FindViewById<EditText>(Resource.Id.edit_Diameter);
                var getPrice = view.FindViewById<EditText>(Resource.Id.edit_Price);
                var getWeight = view.FindViewById<EditText>(Resource.Id.edit_Weight);


                alertbuilder.SetCancelable(false)
                    //ниже код если пользователь нажал OK в диалоговом окне
                    .SetPositiveButton(dialog_add, delegate
                    {
                    ///если поля пустые, то присваиваем переменным значение 0                                   
                    //получаем значение поля getQuantity и присваиваем его переменной quantity (класс Pizza если 0 присвоит 1 в любом случае) 
                    if (getQuantity.Text == "" || getQuantity.Text == null)
                        { quantity = 0; }
                        else { quantity = Convert.ToInt32(getQuantity.Text); }
                    //получаем значение поля getDiameter и присваиваем его переменной diameter
                    if (getDiameter.Text == "" || getDiameter.Text == null)
                        { diameter = 0; }
                        else { double.TryParse(getDiameter.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out diameter); }
                    //получаем значение поля getPrice и присваиваем его переменной price
                    if (getPrice.Text == "" || getPrice.Text == null)
                        { price = 0; }
                        else { double.TryParse(getPrice.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out price); }
                    //получаем значение поля getWeight и присваиваем его переменной weight
                    if (getWeight.Text == "" || getWeight.Text == null)
                        { weight = 0; }
                        else { double.TryParse(getWeight.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out weight); }
                    ///конец сбора введенных в диалоговом окне данных

                    //присваиваем string'овой переменной pictureName текстовое значение (название картинки) из перемешенного масива фоток photolist 
                    string pictureName = photolist[i];
                    //присваиваем параметру pic картинку, используя ее название из перемешенного масива фоток photolist
                    pic = Resources.GetIdentifier(pictureName, "drawable", PackageName);


                    ///из полученных введенных данных и картинки создаем обьект масива pizza
                    pizza.Add(new PizzaList(quantity, diameter, price, weight, pic));
                    //добавляем созданную пиццу с индексом listNumber в масив для recycler adapter
                    PizzaListitems.Add(pizza[listNumber]);
                    //говорим адаптеру, что масив PizzaListitems обновился и надо обновить view
                    recyclerview_adapter.NotifyDataSetChanged();
                    //каждый раз после добавления увеличиваем listNumber на 1, для того чтобы новая пицца добавлялась второй, третьей и тд, а не переписывала первую
                    listNumber++;

                    //также и с фоткой пиццы
                    //если количество обьектов в масиве pizza будет больше чем количество фоток пиццы, то еще раз перемешиваем масив фоток photolist
                    if (i < photolist.Length - 1)
                        {
                            i++; //следующая фотка из списка
                    }
                        else
                        {
                            rnd = new Random();
                            photolist = photolist.OrderBy(s => rnd.Next()).ToArray();//перемешивем масив фоток photolist
                        i = 0; //индекс возвращаем к 0
                    }

                    //количество пицц в масиве pizza
                    count = pizza.Count();

                    //когда добавили пиццу, и если она первая в recyclerview
                    if (count <= 1)
                        {
                            nodata.Visibility = ViewStates.Invisible; //выключаем текст-подсказку nodata
                        fabCompare.Show(); //показываем кнопку сравнить fabCompare
                        recyclerview.Visibility = ViewStates.Visible; //показываем recyclerview
                    }

                    })
                    .SetNegativeButton(dialog_canc, delegate //кнопка отменить диалоговое окно
                {
                        alertbuilder.Dispose();
                    });
                Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
                dialog.Show();
            };
            ///конец диалогового окна и добавления пицы в recyclerview
            
            //определяем adapter и layout manager для recyclerview, если добавить до кнопки добавить то не работает что-то, хз почему
            recyclerview_layoutmanger = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerview.SetLayoutManager(recyclerview_layoutmanger);
            recyclerview_adapter = new RecyclerAdapter(PizzaListitems, this);
            recyclerview.SetAdapter(recyclerview_adapter);
            //нажатие на кнопку сравнить fabCompare
            fabCompare.Click += delegate {
                //запускаем метод, который показывает ProgressBar 
                ShowProgressBar(true);
                //в новом потоке (что бы интерфейс не лагал) запускаем "нажатие" кнопки
                ThreadStart th = new ThreadStart(Button_Click);
                Thread myThread = new Thread(th);
                myThread.Start();
            };

            //метод "нажатия" кнопки fabCompare, который запускается новым потоком
            void Button_Click()
            {
                //с помощью JSON передаем масив pizza и запускаем CompareActivity
                var intent = new Intent(this, typeof(CompareActivity));
                string listAsString = JsonConvert.SerializeObject(pizza);
                intent.PutExtra("saved_counter", listAsString);
                this.StartActivity(intent);
            }

            ///конец OnCreate
        }

        //метод для контекстного меню "Удалить" position и num передаем из recyclerview_adapter
        public void Delete(int position, int num)
        {
            pizza.RemoveAt(position);//удаляем объект масива pizza
            listNumber--;//отнимаем -1 от listNumber, иначе после удаления пиццы мы не сможем создать новую, индекс в масиве будет неверный
            Toast.MakeText(this, piz_num + num.ToString() + toast_del, ToastLength.Short).Show(); //сообщение о том что пицца № num удалена

            //количество пицц в масиве pizza
            count = pizza.Count();

            ///если во время удаления выполняется одно из условий - выполняем
            //если count = 0, значит мы удалили последнюю пиццу из масива pizza
            if (count == 0)
            {
                nodata.Visibility = ViewStates.Visible;//включаем текст-подсказку nodata 
                fabCompare.Hide();//прячем кнопку сравнить fabCompare
                recyclerview.Visibility = ViewStates.Invisible;//прячем recyclerview
            }
            //если count = 2, мы могли пролистать recyclerview из 3+ пицц и спрятать кнопки fabAdd и fabCompare, а потом удалить одну из пицц, кнопки тогда не появятся так как листать нечего, все влазит на экран. Что бы быть увереным, когда остается 2 пиццы показываем кнопки принудительно
            else if (count == 2)
            {
                fabAdd.Show();
                fabCompare.Show();
            }
        }

        //метод для контекстного меню "Редактировать" position и num передаем из recyclerview_adapter
        public void Edit(int position, int num)
        {
            //создаем для диалогового окна новый view и присваиваем ему layout с именем user_input_dialog_box
            LayoutInflater layoutInflater = LayoutInflater.From(this);
            View view = layoutInflater.Inflate(Resource.Layout.user_input_dialog_box, null);
            Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertbuilder.SetView(view);

            //создаем переменные и присваиваем им элементы из layout с именем user_input_dialog_box
            var getQuantity = view.FindViewById<EditText>(Resource.Id.edit_Quantity);
            var getDiameter = view.FindViewById<EditText>(Resource.Id.edit_Diameter);
            var getPrice = view.FindViewById<EditText>(Resource.Id.edit_Price);
            var getWeight = view.FindViewById<EditText>(Resource.Id.edit_Weight);

            //присваиваем переменным значения пиццы в позиции position масива pizza
            getQuantity.Text = Convert.ToString(pizza[position].quantity);
            getDiameter.Text = Convert.ToString(pizza[position].diameter);
            getPrice.Text = Convert.ToString(pizza[position].price);
            getWeight.Text = Convert.ToString(pizza[position].weight);

            alertbuilder.SetCancelable(false)
            //ниже код если пользователь нажал OK в диалоговом окне
                .SetPositiveButton(dialog_ok, delegate
                {

                    ///если поля пустые, то присваиваем переменным значение 0                                   
                    //получаем значение поля getQuantity и присваиваем его переменной quantity (класс Pizza если 0 присвоит 1 в любом случае) 
                    if (getQuantity.Text == "" || getQuantity.Text == null)
                    { quantity = 0; }
                    else { quantity = Convert.ToInt32(getQuantity.Text); }
                    //получаем значение поля getDiameter и присваиваем его переменной diameter
                    if (getDiameter.Text == "" || getDiameter.Text == null)
                    { diameter = 0; }
                    else { double.TryParse(getDiameter.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out diameter); }
                    //получаем значение поля getPrice и присваиваем его переменной price
                    if (getPrice.Text == "" || getPrice.Text == null)
                    { price = 0; }
                    else { double.TryParse(getPrice.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out price); }
                    //получаем значение поля getWeight и присваиваем его переменной weight
                    if (getWeight.Text == "" || getWeight.Text == null)
                    { weight = 0; }
                    else { double.TryParse(getWeight.Text, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out weight); }
                    ///конец сбора введенных в диалоговом окне данных

                    //присваиваем пицце в позиции position масива pizza значение выше собранных переменных
                    pizza[position].Quantity = quantity;
                    pizza[position].diameter = diameter;
                    pizza[position].price = price;
                    pizza[position].weight = weight;

                    //сообщение о том что пицца № num изменена
                    Toast.MakeText(this, piz_num + num.ToString() + toast_edit, ToastLength.Short).Show();
                    //говорим recyclerview_adapter что пицца в позиции position масива pizza изменена, он обновит значения в receclerview
                    recyclerview_adapter.NotifyItemChanged(position);

                })
                //кнопка отменить диалоговое окно
                .SetNegativeButton(dialog_canc, delegate
                {
                    alertbuilder.Dispose();
                });
            Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
            dialog.Show();
        }
        //метод для показа ProgressBar
        void ShowProgressBar(bool show)
        {
            RunOnUiThread(() => {
                probar.Visibility = show ? ViewStates.Visible : ViewStates.Invisible;
            });
        }
        //переписанный метод OnStop, который выключает ProgressBar
        protected override void OnStop()
        {
            ShowProgressBar(false);
            base.OnStop();
        }

    }
}