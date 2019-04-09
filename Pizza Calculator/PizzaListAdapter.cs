using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.Widget;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Globalization;
using Android.Content.Res;



namespace Pizza_Calculator
{

    public class PizzaListAdapter<T>
    {
        List<T> mItems;
        RecyclerView.Adapter mAdapter;
        public PizzaListAdapter()
        {
            mItems = new List<T>();
        }
        public RecyclerView.Adapter Adapter
        {
            get
            {
                return mAdapter;
            }
            set
            {
                mAdapter = value;
            }
        }
        public void Add(T item)
        {
            mItems.Add(item);
            if (Adapter != null)
            {
                Adapter.NotifyItemInserted(0);
            }
        }
        public void Remove(int position)
        {
            mItems.RemoveAt(position);
            if (Adapter != null)
            {
                Adapter.NotifyItemRemoved(position);
            }
        }
        public T this[int index]
        {
            get
            {
                return mItems[index];
            }
            set
            {
                mItems[index] = value;
            }
        }
        public int Count
        {
            get
            {
                return mItems.Count;
            }
        }
    }

    public class RecyclerAdapter : RecyclerView.Adapter
    {
        private PizzaListAdapter<PizzaList> Mitems;
        Context mContext;
        public RecyclerAdapter(PizzaListAdapter<PizzaList> Mitems, Context context)
        {
            this.Mitems = Mitems;
            NotifyDataSetChanged();
            this.mContext = context;
        }

        public class MyView : RecyclerView.ViewHolder
        {
            public View mainview
            {
                get;
                set;
            }
            public TextView mpizza_number_value
            {
                get;
                set;
            }
            public TextView mpizza_quantity_value
            {
                get;
                set;
            }
            public TextView mpizza_diameter_value
            {
                get;
                set;
            }
            public TextView mpizza_price_value
            {
                get;
                set;
            }
            public TextView mpizza_weight_value
            {
                get;
                set;
            }

            public TextView mpizza_edge_info
            {
                get;
                set;
            }

            public ImageView mpicture
            {
                get;
                set;
            }

            public Button mmenu;
                                   
            public MyView(View view) : base(view)
            {
                mainview = view;
            }
        }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View listitem = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PizzaListLayout, parent, false);
            TextView pizza_number_value = listitem.FindViewById<TextView>(Resource.Id.pizza_number_value);
            TextView pizza_quantity_value = listitem.FindViewById<TextView>(Resource.Id.pizza_quantity_value);
            TextView pizza_diameter_value = listitem.FindViewById<TextView>(Resource.Id.pizza_diameter_value);
            TextView pizza_price_value = listitem.FindViewById<TextView>(Resource.Id.pizza_price_value);
            TextView pizza_weight_value = listitem.FindViewById<TextView>(Resource.Id.pizza_weight_value);
            TextView pizza_edge_info = listitem.FindViewById<TextView>(Resource.Id.InfoEdge);
            ImageView Pizza_image = listitem.FindViewById<ImageView>(Resource.Id.Pizza_image);
            Button menu = listitem.FindViewById<Button>(Resource.Id.menuButton);


            MyView view = new MyView(listitem)
            {
                mpizza_number_value = pizza_number_value,
                mpizza_quantity_value = pizza_quantity_value,
                mpizza_diameter_value = pizza_diameter_value,
                mpizza_price_value = pizza_price_value,
                mpizza_weight_value = pizza_weight_value,
                mpizza_edge_info = pizza_edge_info,
                mpicture = Pizza_image,
                mmenu = menu
            };

            //ниже кусок с кнопкой
            view.mmenu.Click += (o, e) =>
            {

            Android.Widget.PopupMenu popup = new Android.Widget.PopupMenu(view.mmenu.Context, view.mmenu);
            // Call inflate directly on the menu:
            popup.Inflate(Resource.Menu.options_menu);
                
            popup.MenuItemClick += (s, args) =>
            {
                switch (args.Item.ItemId)
                {
                    case Resource.Id.delete:
                        int num = view.AdapterPosition+1;                        
                        Mitems.Remove(view.AdapterPosition);
                        ((MainActivity)mContext).Delete(view.AdapterPosition, num);
                        NotifyItemRemoved(view.AdapterPosition);
                        break;

                    case Resource.Id.edit:
                        num = view.AdapterPosition + 1;
                        ((MainActivity)mContext).Edit(view.AdapterPosition, num);
                        break;
                }
            };
            popup.Show();
            };
            //тут кусок кнопки заканчивается

            return view;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyView myholder = holder as MyView;
            myholder.mpizza_number_value.Text = Convert.ToString(position + 1);
            myholder.mpizza_quantity_value.Text = Convert.ToString(Mitems[position].Quantity);
            myholder.mpizza_diameter_value.Text = Mitems[position].diameter.ToString(CultureInfo.InvariantCulture);
            myholder.mpizza_price_value.Text = Mitems[position].price.ToString(CultureInfo.InvariantCulture);
            myholder.mpizza_weight_value.Text = Mitems[position].weight.ToString(CultureInfo.InvariantCulture);
            myholder.mpicture.SetImageResource(Mitems[position].picture);

            string info="";

            if (Mitems[position].edge != 0)
            {
                info = ((MainActivity)mContext).Info(position, info);
               // info = "lol test"; //Resources.GetText(Resource.String.InfoEdge);
            }

            myholder.mpizza_edge_info.Text = info;

        }
               
        public override int ItemCount
        {
            get
            {
                return Mitems.Count;
            }
        }
    }
}