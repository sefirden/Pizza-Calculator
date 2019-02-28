﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Pizza_Calculator
{
    public class PizzaList
    {
        public int quantity;//количество pizz
        public int Quantity//если количество введено 0 или меньше, то автоматически считает как для 1
        {
            get { return quantity; }
            set
            {
                if (value <= 0)
                    quantity = 1;
                else
                    quantity = value;
            }
        }
        public double diameter
        {
            get;
            set;
        }   //диаметр
        public double price
        {
            get;
            set;
        }   //цена
        public double weight
        {
            get;
            set;
        }   //вес

        public int picture
        {
            get;
            set;
        }

        public PizzaList(int q, double d, double p, double w, int pic)//в таком порядке заполняем данные для пиццы
        {
            Quantity = q;
            diameter = d;
            price = p;
            weight = w;
            picture = pic;
        }
        //считаем площадь пиццы
        public double GetArea()
        {
                return (diameter <= 0) ? 0 : Math.Round(Quantity * ((Math.Pow(diameter, 2) * Math.PI) / 40000), 4);

        }
        //считаем блину борта, для тех пицц где он бывает с сыром или мясом
        public double GetEdgeLength()
        {
            return (diameter <= 0) ? 0 : Math.Round(Quantity * (Math.PI * diameter), 2);//см выше про  GetArea
        }
        //считаем соотношение цены к площади, сколько $ надо заплатить за м²
        public double PriceToArea()
        {
            double x = GetArea();
            return (price <= 0) ? 0 : Math.Round((Quantity * price) / x, 2);//см выше про GetArea
        }
        //считаем соотношение цены к весу, сколько $ надо заплатить за 1 кг
        public double PriceToWeight()
        {
            if (price <= 0 || weight <= 0)
            {
                return 0;
            }
            else
            {
                return Math.Round((price / weight) * 1000, 2);//см выше про GetArea
            }
        }
    }
}