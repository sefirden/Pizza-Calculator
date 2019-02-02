﻿using System.Collections.Generic;
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

namespace Pizza_Calculator
{
    public class PizzaListAdapter<T>
    {
        readonly List<T> mItems;
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
                Adapter.NotifyItemRemoved(0);
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
        Context context;
        public RecyclerAdapter(PizzaListAdapter<PizzaList> Mitems)
        {
            this.Mitems = Mitems;
            NotifyDataSetChanged();
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

            MyView view = new MyView(listitem)
            {
                mpizza_number_value = pizza_number_value,
                mpizza_quantity_value = pizza_quantity_value,
                mpizza_diameter_value = pizza_diameter_value,
                mpizza_price_value = pizza_price_value,
                mpizza_weight_value = pizza_weight_value
            };
            return view;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyView myholder = holder as MyView;
            myholder.mpizza_number_value.Text = Convert.ToString(position + 1);
            myholder.mpizza_quantity_value.Text = Convert.ToString(Mitems[position].Quantity);
            myholder.mpizza_diameter_value.Text = Convert.ToString(Mitems[position].diameter);
            myholder.mpizza_price_value.Text = Convert.ToString(Mitems[position].price);
            myholder.mpizza_weight_value.Text = Convert.ToString(Mitems[position].weight);
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