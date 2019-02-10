using System;
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
    class CompareList
    {
        public string Name
        {
            get;
            set;
        }
        public double Area //площадь в см2
        {
            get;
            set;
        }   
        public double EdgeLength
        {
            get;
            set;
        }   
        public double PriceToArea
        {
            get;
            set;
        }   
        public double PriceToWeight
        {
            get;
            set;
        }
        public CompareList(string n, double a, double el, double pa, double pw)//в таком порядке заполняем данные для пиццы
        {
            Name = n;
            Area = a;
            EdgeLength = el;
            PriceToArea = pa;
            PriceToWeight = pw;
        }
    }
}